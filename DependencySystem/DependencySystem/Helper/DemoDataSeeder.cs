using DependencySystem.DAL;
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Helper
{
    public static class DemoDataSeeder
    {
        public static async Task SeedDemoDataAsync(IServiceProvider services, IConfiguration config)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // ✅ Ensure DB
            await context.Database.MigrateAsync();

            // ✅ Roles
            string[] roles = { "Admin", "Manager", "Developer", "Maintainer" };
            foreach (var r in roles)
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));

            // ✅ Create demo users
            async Task<ApplicationUser> CreateUser(string email, string username, string password, string role)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null) return user;

                user = new ApplicationUser
                {
                    Email = email,
                    UserName = username,
                    EmailConfirmed = true,
                    IsVerified = true
                };

                var res = await userManager.CreateAsync(user, password);
                if (!res.Succeeded)
                    throw new Exception("User create failed: " + string.Join(" | ", res.Errors.Select(e => e.Description)));

                await userManager.AddToRoleAsync(user, role);
                return user;
            }

            var demo = config.GetSection("DemoUsers");

            var admin = await CreateUser(demo["Admin:Email"]!, demo["Admin:Username"]!, demo["Admin:Password"]!, "Admin");
            var admin1 = await CreateUser(demo["Admin:Email"]!, demo["Admin:Username"]!, demo["Admin:Password"]!, "Admin");
            var manager = await CreateUser(demo["Manager:Email"]!, demo["Manager:Username"]!, demo["Manager:Password"]!, "Manager");
            var dev1 = await CreateUser(demo["Developer1:Email"]!, demo["Developer1:Username"]!, demo["Developer1:Password"]!, "Developer");
            var dev2 = await CreateUser(demo["Developer2:Email"]!, demo["Developer2:Username"]!, demo["Developer2:Password"]!, "Developer");
            var dev3 = await CreateUser(demo["Developer3:Email"]!, demo["Developer3:Username"]!, demo["Developer3:Password"]!, "Developer");
            var maintainer = await CreateUser(demo["Maintainer:Email"]!, demo["Maintainer:Username"]!, demo["Maintainer:Password"]!, "Maintainer");

            // ✅ If already seeded then skip
            if (await context.Companies.AnyAsync()) return;

            // ============================
            // COMPANY + DEPARTMENT
            // ============================
            var c1 = new Company { CompanyName = "DAC Software Pvt Ltd" };
            var c2 = new Company { CompanyName = "Dependency Labs India" };
            context.Companies.AddRange(c1, c2);
            await context.SaveChangesAsync();

            var d1 = new Department { DepartmentName = "Backend Team", CompanyID = c1.CompanyID };
            var d2 = new Department { DepartmentName = "Frontend Team", CompanyID = c1.CompanyID };
            var d3 = new Department { DepartmentName = "DevOps Team", CompanyID = c2.CompanyID };
            var d4 = new Department { DepartmentName = "QA Team", CompanyID = c2.CompanyID };

            context.Departments.AddRange(d1, d2, d3, d4);
            await context.SaveChangesAsync();

            // ============================
            // PROJECTS
            // ============================
            var p1 = new Project { ProjectName = "DependencySystem API", DepartmentID = d1.DepartmentID };
            var p2 = new Project { ProjectName = "DependencySystem UI", DepartmentID = d2.DepartmentID };
            var p3 = new Project { ProjectName = "Risk Dashboard", DepartmentID = d4.DepartmentID };
            var p4 = new Project { ProjectName = "CI/CD Pipeline", DepartmentID = d3.DepartmentID };

            context.Projects.AddRange(p1, p2, p3, p4);
            await context.SaveChangesAsync();

            // ============================
            // MODULES (10)
            // ============================
            var modules = new List<Module>
            {
                new Module{ ModuleName="Auth Module", ProjectID=p1.ProjectID, Status="InProgress" },
                new Module{ ModuleName="Organization Module", ProjectID=p1.ProjectID, Status="Pending" },
                new Module{ ModuleName="Project Module", ProjectID=p1.ProjectID, Status="Pending" },
                new Module{ ModuleName="Dependency Graph", ProjectID=p1.ProjectID, Status="Pending" },
                new Module{ ModuleName="Team Management", ProjectID=p1.ProjectID, Status="Pending" },

                new Module{ ModuleName="Login UI", ProjectID=p2.ProjectID, Status="InProgress" },
                new Module{ ModuleName="Dashboard UI", ProjectID=p2.ProjectID, Status="Pending" },
                new Module{ ModuleName="Project Pages UI", ProjectID=p2.ProjectID, Status="Pending" },

                new Module{ ModuleName="Risk Analytics", ProjectID=p3.ProjectID, Status="Pending" },
                new Module{ ModuleName="Deployment Setup", ProjectID=p4.ProjectID, Status="Pending" }
            };

            context.Modules.AddRange(modules);
            await context.SaveChangesAsync();

            // ============================
            // TASKS (20)
            // ============================
            var tasks = new List<TaskEntity>();
            int i = 1;
            foreach (var m in modules)
            {
                tasks.Add(new TaskEntity { Title = $"Task {i++}: Setup for {m.ModuleName}", Description = "Initial setup and validation", ModuleID = m.ModuleID, Status = "Pending" });
                tasks.Add(new TaskEntity { Title = $"Task {i++}: Implement CRUD for {m.ModuleName}", Description = "Add APIs and services", ModuleID = m.ModuleID, Status = "Pending" });
            }

            context.Tasks.AddRange(tasks.Take(20));
            await context.SaveChangesAsync();

            // ============================
            // MODULE DEPENDENCIES (10)
            // ============================
            var depList = new List<Dependency>
            {
                new Dependency{ SourceModuleID=modules[1].ModuleID, TargetModuleID=modules[0].ModuleID }, // Org depends Auth
                new Dependency{ SourceModuleID=modules[2].ModuleID, TargetModuleID=modules[1].ModuleID }, // Project depends Org
                new Dependency{ SourceModuleID=modules[3].ModuleID, TargetModuleID=modules[2].ModuleID }, // Dependency depends Project
                new Dependency{ SourceModuleID=modules[4].ModuleID, TargetModuleID=modules[2].ModuleID }, // Team depends Project

                new Dependency{ SourceModuleID=modules[6].ModuleID, TargetModuleID=modules[5].ModuleID }, // Dashboard depends Login UI
                new Dependency{ SourceModuleID=modules[7].ModuleID, TargetModuleID=modules[6].ModuleID },

                new Dependency{ SourceModuleID=modules[8].ModuleID, TargetModuleID=modules[3].ModuleID }, // Risk depends DepGraph
                new Dependency{ SourceModuleID=modules[9].ModuleID, TargetModuleID=modules[0].ModuleID }  // Deploy depends Auth
            };

            context.Dependencies.AddRange(depList);
            await context.SaveChangesAsync();

            // ============================
            // TASK DEPENDENCIES (10)
            // ============================
            var tdeps = new List<TaskDependency>();
            for (int t = 2; t <= 20; t += 2)
            {
                tdeps.Add(new TaskDependency
                {
                    TaskID = tasks[t - 1].TaskID,            // CRUD task
                    DependsOnTaskID = tasks[t - 2].TaskID     // Setup task
                });
            }
            context.TaskDependencies.AddRange(tdeps);
            await context.SaveChangesAsync();

            // ============================
            // TECHNOLOGIES (10)
            // ============================
            var techs = new List<Technology>
            {
                new Technology{ TechnologyName="ASP.NET Core" },
                new Technology{ TechnologyName="EF Core" },
                new Technology{ TechnologyName="MySQL" },
                new Technology{ TechnologyName="JWT" },
                new Technology{ TechnologyName="Swagger" },
                new Technology{ TechnologyName="React" },
                new Technology{ TechnologyName="Docker" },
                new Technology{ TechnologyName="GitHub Actions" },
                new Technology{ TechnologyName="SMTP" },
                new Technology{ TechnologyName="Mermaid Diagrams" },
            };
            context.Technologies.AddRange(techs);
            await context.SaveChangesAsync();

            // project tech mapping
            context.ProjectTechnologies.AddRange(
                new ProjectTechnology { ProjectID = p1.ProjectID, TechnologyID = techs[0].TechnologyID },
                new ProjectTechnology { ProjectID = p1.ProjectID, TechnologyID = techs[1].TechnologyID },
                new ProjectTechnology { ProjectID = p1.ProjectID, TechnologyID = techs[2].TechnologyID },
                new ProjectTechnology { ProjectID = p1.ProjectID, TechnologyID = techs[3].TechnologyID },

                new ProjectTechnology { ProjectID = p2.ProjectID, TechnologyID = techs[5].TechnologyID },
                new ProjectTechnology { ProjectID = p4.ProjectID, TechnologyID = techs[7].TechnologyID }
            );

            // user tech mapping
            context.UserTechnologies.AddRange(
                new UserTechnology { UserID = dev1.Id, TechnologyID = techs[5].TechnologyID },
                new UserTechnology { UserID = dev2.Id, TechnologyID = techs[0].TechnologyID },
                new UserTechnology { UserID = dev3.Id, TechnologyID = techs[1].TechnologyID },
                new UserTechnology { UserID = maintainer.Id, TechnologyID = techs[6].TechnologyID }
            );

            await context.SaveChangesAsync();

            // ============================
            // TEAM PROFILES
            // ============================
            context.TeamMemberProfiles.AddRange(
                new TeamMemberProfile { UserID = manager.Id, DepartmentID = d1.DepartmentID, Designation = "Project Manager", ExperienceYears = 3 },
                new TeamMemberProfile { UserID = dev1.Id, DepartmentID = d1.DepartmentID, Designation = "Backend Dev", ExperienceYears = 1 },
                new TeamMemberProfile { UserID = dev2.Id, DepartmentID = d2.DepartmentID, Designation = "Frontend Dev", ExperienceYears = 1 },
                new TeamMemberProfile { UserID = dev3.Id, DepartmentID = d4.DepartmentID, Designation = "QA Engineer", ExperienceYears = 1 },
                new TeamMemberProfile { UserID = maintainer.Id, DepartmentID = d3.DepartmentID, Designation = "DevOps Engineer", ExperienceYears = 2 }
            );

            await context.SaveChangesAsync();

            // ============================
            // PROJECT TEAM MEMBERS (Project Roles)
            // ============================
            context.ProjectTeamMembers.AddRange(
                new ProjectTeamMember { ProjectID = p1.ProjectID, UserID = manager.Id, Role = "Manager" },
                new ProjectTeamMember { ProjectID = p1.ProjectID, UserID = dev1.Id, Role = "Developer" },
                new ProjectTeamMember { ProjectID = p2.ProjectID, UserID = dev2.Id, Role = "Developer" },
                new ProjectTeamMember { ProjectID = p3.ProjectID, UserID = dev3.Id, Role = "Developer" },
                new ProjectTeamMember { ProjectID = p4.ProjectID, UserID = maintainer.Id, Role = "Maintainer" }
            );

            await context.SaveChangesAsync();
        }
    }
}
