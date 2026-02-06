using DependencySystem.DAL;
using DependencySystem.Helper;
using DependencySystem.Models;
using DependencySystem.Models.enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Seeding
{
    public static class RelationalDemoSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration config)
        {
            if (!config.GetValue<bool>("RelationalSeeding:Enable"))
            {
                Console.WriteLine("⛔ Relational seeding disabled");
                return;
            }

            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();
            Console.WriteLine("🚀 Relational seeding started");

            // ================= ROLES =================
            string[] roles = { AppRoles.Admin, AppRoles.Manager, AppRoles.Developer };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ================= USERS =================
            async Task<ApplicationUser> CreateUser(string email, string role)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null) return user;

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    IsVerified = true
                };

                await userManager.CreateAsync(user, "Admin@12345");
                await userManager.AddToRoleAsync(user, role);
                return user;
            }

            var admin = await CreateUser("admin@test.com", AppRoles.Admin);
            var manager = await CreateUser("manager@test.com", AppRoles.Manager);
            var dev = await CreateUser("dev@test.com", AppRoles.Developer);

            // ================= CONFIG =================
            int companyCount = config.GetValue<int>("RelationalSeeding:Companies");
            int deptPerCompany = config.GetValue<int>("RelationalSeeding:DepartmentsPerCompany");
            int projPerDept = config.GetValue<int>("RelationalSeeding:ProjectsPerDepartment");
            int modPerProj = config.GetValue<int>("RelationalSeeding:ModulesPerProject");
            int taskPerMod = config.GetValue<int>("RelationalSeeding:TasksPerModule");

            // ================= COMPANIES =================
            if (!await context.Companies.AnyAsync())
            {
                var companies = Enumerable.Range(1, companyCount)
                    .Select(i => new Company { CompanyName = $"Company-{i}" })
                    .ToList();

                context.Companies.AddRange(companies);
                await context.SaveChangesAsync();
            }

            // ================= DEPARTMENTS =================
            if (!await context.Departments.AnyAsync())
            {
                var companies = await context.Companies.ToListAsync();
                var departments = companies.SelectMany(c =>
                    Enumerable.Range(1, deptPerCompany)
                        .Select(i => new Department
                        {
                            DepartmentName = $"{c.CompanyName}-Dept-{i}",
                            CompanyID = c.CompanyID
                        })
                );

                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }

            // ================= PROJECTS =================
            if (!await context.Projects.AnyAsync())
            {
                var depts = await context.Departments.ToListAsync();
                var projects = depts.SelectMany(d =>
                    Enumerable.Range(1, projPerDept)
                        .Select(i => new Project
                        {
                            ProjectName = $"{d.DepartmentName}-Project-{i}",
                            DepartmentID = d.DepartmentID
                        })
                );

                context.Projects.AddRange(projects);
                await context.SaveChangesAsync();
            }

            // ================= MODULES =================
            if (!await context.Modules.AnyAsync())
            {
                var projects = await context.Projects.ToListAsync();
                var modules = projects.SelectMany(p =>
                    Enumerable.Range(1, modPerProj)
                        .Select(i => new Module
                        {
                            ModuleName = $"{p.ProjectName}-Module-{i}",
                            Status = ModuleStatus.Pending,
                            ProjectID = p.ProjectID
                        })
                );

                context.Modules.AddRange(modules);
                await context.SaveChangesAsync();
            }

            // ================= TASKS =================
            if (!await context.Tasks.AnyAsync())
            {
                var modules = await context.Modules.ToListAsync();
                var tasks = modules.SelectMany(m =>
                    Enumerable.Range(1, taskPerMod)
                        .Select(i => new TaskEntity
                        {
                            Title = $"{m.ModuleName}-Task-{i}",
                            Status = TaskStatuss.Pending,
                            ModuleID = m.ModuleID
                        })
                );

                context.Tasks.AddRange(tasks);
                await context.SaveChangesAsync();
            }

            // ================= MODULE DEPENDENCIES (DAG) =================
            if (!await context.Dependencies.AnyAsync())
            {
                var modules = await context.Modules
                    .OrderBy(m => m.ModuleID)
                    .ToListAsync();

                var deps = new List<Dependency>();

                for (int i = 1; i < modules.Count; i++)
                {
                    if (i % 3 == 0)
                    {
                        deps.Add(new Dependency
                        {
                            SourceModuleID = modules[i - 1].ModuleID,
                            TargetModuleID = modules[i].ModuleID
                        });
                    }
                }

                context.Dependencies.AddRange(deps);
                await context.SaveChangesAsync();
            }
            // ================= MODULE DEPENDENCIES =================
            if (!await context.Dependencies.AnyAsync())
            {
                var modulesByProject = await context.Modules
                    .GroupBy(m => m.ProjectID)
                    .ToListAsync();

                var dependencies = new List<Dependency>();

                foreach (var projectModules in modulesByProject)
                {
                    var ordered = projectModules
                        .OrderBy(m => m.ModuleID)
                        .ToList();

                    // Linear DAG: M1 → M2 → M3
                    for (int i = 1; i < ordered.Count; i++)
                    {
                        dependencies.Add(new Dependency
                        {
                            SourceModuleID = ordered[i].ModuleID,
                            TargetModuleID = ordered[i - 1].ModuleID
                        });
                    }
                }

                context.Dependencies.AddRange(dependencies);
                await context.SaveChangesAsync();
            }


            // ================= TASK DEPENDENCIES (DAG) =================
            if (!await context.TaskDependencies.AnyAsync())
            {
                var tasks = await context.Tasks
                    .OrderBy(t => t.TaskID)
                    .ToListAsync();

                var taskDeps = new List<TaskDependency>();

                for (int i = 1; i < tasks.Count; i++)
                {
                    if (i % 4 == 0)
                    {
                        taskDeps.Add(new TaskDependency
                        {
                            TaskID = tasks[i].TaskID,
                            DependsOnTaskID = tasks[i - 1].TaskID
                        });
                    }
                }

                context.TaskDependencies.AddRange(taskDeps);
                await context.SaveChangesAsync();
            }
            // ================= TASK DEPENDENCIES =================
            if (!await context.TaskDependencies.AnyAsync())
            {
                var tasksByModule = await context.Tasks
                    .GroupBy(t => t.ModuleID)
                    .ToListAsync();

                var taskDependencies = new List<TaskDependency>();

                foreach (var moduleTasks in tasksByModule)
                {
                    var ordered = moduleTasks
                        .OrderBy(t => t.TaskID)
                        .ToList();

                    // Linear DAG: T1 → T2 → T3
                    for (int i = 1; i < ordered.Count; i++)
                    {
                        taskDependencies.Add(new TaskDependency
                        {
                            TaskID = ordered[i].TaskID,
                            DependsOnTaskID = ordered[i - 1].TaskID
                        });
                    }
                }

                context.TaskDependencies.AddRange(taskDependencies);
                await context.SaveChangesAsync();
            }

            Console.WriteLine("✅ RELATIONAL DEV DATA SEEDED (ENTERPRISE SCALE)");
        }
    }
}
