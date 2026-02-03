using DependencySystem.Helper;
using Microsoft.AspNetCore.Identity;

namespace DependencySystem.Helper
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles =
            {
                AppRoles.Admin,
                AppRoles.Manager,
                AppRoles.Developer,
                AppRoles.Maintainer
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
