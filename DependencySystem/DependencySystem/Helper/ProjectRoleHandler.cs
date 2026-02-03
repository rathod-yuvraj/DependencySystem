using DependencySystem.DAL;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DependencySystem.Helper
{
    public class ProjectRoleHandler : AuthorizationHandler<ProjectRoleRequirement>
    {
        private readonly ApplicationDbContext _context;

        public ProjectRoleHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProjectRoleRequirement requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            // ✅ ProjectId should be passed via route data: /api/project/{projectId}/...
            if (context.Resource is not HttpContext httpContext) return;

            if (!httpContext.Request.RouteValues.TryGetValue("projectId", out var projectIdObj))
                return;

            if (!int.TryParse(projectIdObj?.ToString(), out int projectId))
                return;

            // ✅ Admin always allowed
            if (context.User.IsInRole(AppRoles.Admin))
            {
                context.Succeed(requirement);
                return;
            }

            // ✅ Check project team member role
            var memberRole = await _context.ProjectTeamMembers
                .Where(x => x.ProjectID == projectId && x.UserID == userId)
                .Select(x => x.Role)
                .FirstOrDefaultAsync();

            if (memberRole == null) return;

            if (requirement.AllowedRoles.Contains(memberRole))
            {
                context.Succeed(requirement);
            }
        }
    }
}
