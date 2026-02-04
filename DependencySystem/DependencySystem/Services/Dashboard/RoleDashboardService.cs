using DependencySystem.DAL;
using DependencySystem.DTOs.Dashboard;
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Dashboard
{
    public class RoleDashboardService : IRoleDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleDashboardService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ===================== ADMIN =====================
        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            return new AdminDashboardDto
            {
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalDepartments = await _context.Departments.CountAsync(),
                TotalProjects = await _context.Projects.CountAsync(),
                TotalUsers = _userManager.Users.Count(),
                TotalBlockedTasks = await _context.TaskDependencies.CountAsync()
            };
        }

        // ===================== MANAGER =====================
        public async Task<ManagerDashboardDto> GetManagerDashboardAsync(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.Modules)
                    .ThenInclude(m => m.Tasks)
                .FirstAsync(p => p.ProjectID == projectId);

            var modules = project.Modules;
            var tasks = modules.SelectMany(m => m.Tasks).ToList();

            var completedTasks = tasks.Count(t => t.Status == "Completed");

            return new ManagerDashboardDto
            {
                ProjectId = project.ProjectID,
                ProjectName = project.ProjectName,

                TotalModules = modules.Count,
                CompletedModules = modules.Count(m => m.Status == "Completed"),

                TotalTasks = tasks.Count,
                CompletedTasks = completedTasks,
                BlockedTasks = await _context.TaskDependencies
                    .CountAsync(td => td.Task.Module.ProjectID == projectId),

                ProgressPercentage = tasks.Count == 0
                    ? 0
                    : Math.Round((double)completedTasks / tasks.Count * 100, 2)
            };
        }

        // ===================== DEVELOPER =====================
        public async Task<DeveloperDashboardDto> GetDeveloperDashboardAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var tasks = await _context.Tasks
                .Where(t => t.Module!.Project.ProjectTeamMembers
                    .Any(ptm => ptm.UserID == userId))
                .ToListAsync();

            var completed = tasks.Count(t => t.Status == "Completed");

            return new DeveloperDashboardDto
            {
                UserId = int.Parse(userId),
                Username = user!.UserName!,

                TotalAssignedTasks = tasks.Count,
                CompletedTasks = completed,
                PendingTasks = tasks.Count - completed,
                BlockedTasks = await _context.TaskDependencies
                    .CountAsync(td => td.Task.Module.Project.ProjectTeamMembers
                        .Any(ptm => ptm.UserID == userId))
            };
        }
    }
}
