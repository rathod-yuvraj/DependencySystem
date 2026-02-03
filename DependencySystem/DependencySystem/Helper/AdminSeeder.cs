
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DependencySystem.Helper
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // ✅ Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
            }

            var adminSection = config.GetSection("AdminSeed");
            var email = adminSection["Email"];
            var username = adminSection["Username"];
            var password = adminSection["Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return;

            // ✅ Check if admin already exists
            var existingAdmin = await userManager.FindByEmailAsync(email);
            if (existingAdmin != null) return;

            var adminUser = new ApplicationUser
            {
                Email = email,
                UserName = username,
                EmailConfirmed = true,
                IsVerified = true
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (!result.Succeeded)
                throw new Exception("Admin seed failed: " + string.Join(" | ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
    }
}
