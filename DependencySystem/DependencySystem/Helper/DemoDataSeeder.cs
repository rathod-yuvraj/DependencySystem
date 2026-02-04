using DependencySystem.DAL;
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Helper
{
    public static class DemoDataSeeder
    {
        public static async Task SeedDemoDataAsync(
            IServiceProvider services,
            IConfiguration config)
        {

            Console.WriteLine("-------------------------------------🚀 DemoDataSeeder started------------------------------------------------------------------------------");

            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // ============================
            // ENSURE DB
            // ============================
            await context.Database.MigrateAsync();

            // ============================
            // ROLES
            // ============================
            string[] roles = { "Admin", "Manager", "Developer", "Maintainer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ============================
            // CREATE USERS (ENV PASSWORDS)
            // ============================
            async Task<ApplicationUser> CreateUser(
                string email,
                string username,
                string password,
                string role)
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

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(" | ",
                        result.Errors.Select(e => e.Description)));

                await userManager.AddToRoleAsync(user, role);
                return user;
            }

            // ============================
            // READ CONFIG (NON-SECRETS)
            // ============================
            var admin = await CreateUser(
                config["DemoUsers:Admin:Email"]!,
                config["DemoUsers:Admin:Username"]!,
                Environment.GetEnvironmentVariable("DEMO_ADMIN_PASSWORD")!,
                "Admin");

            var manager = await CreateUser(
                config["DemoUsers:Manager:Email"]!,
                config["DemoUsers:Manager:Username"]!,
                Environment.GetEnvironmentVariable("DEMO_MANAGER_PASSWORD")!,
                "Manager");

            var dev1 = await CreateUser(
                config["DemoUsers:Developer1:Email"]!,
                config["DemoUsers:Developer1:Username"]!,
                Environment.GetEnvironmentVariable("DEMO_DEV_PASSWORD")!,
                "Developer");

            var dev2 = await CreateUser(
                config["DemoUsers:Developer2:Email"]!,
                config["DemoUsers:Developer2:Username"]!,
                Environment.GetEnvironmentVariable("DEMO_DEV_PASSWORD")!,
                "Developer");

            var dev3 = await CreateUser(
                config["DemoUsers:Developer3:Email"]!,
                config["DemoUsers:Developer3:Username"]!,
                Environment.GetEnvironmentVariable("DEMO_DEV_PASSWORD")!,
                "Developer");

            var maintainer = await CreateUser(
                config["DemoUsers:Maintainer:Email"]!,
                config["DemoUsers:Maintainer:Username"]!,
                Environment.GetEnvironmentVariable("DEMO_MAINTAINER_PASSWORD")!,
                "Maintainer");

            // ============================
            // STOP IF DATA EXISTS
            // ============================
            if (await context.Companies.AnyAsync())
                return;

            // ============================
            // COMPANIES
            // ============================
            var c1 = new Company { CompanyName = "DAC Software Pvt Ltd" };
            var c2 = new Company { CompanyName = "Dependency Labs India" };
            context.Companies.AddRange(c1, c2);
            await context.SaveChangesAsync();

            // ============================
            // DEPARTMENTS
            // ============================
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
        }
    }
}
