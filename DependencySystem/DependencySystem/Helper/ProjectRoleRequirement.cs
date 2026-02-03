using Microsoft.AspNetCore.Authorization;

namespace DependencySystem.Helper
{
    public class ProjectRoleRequirement : IAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }

        public ProjectRoleRequirement(params string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }
}
