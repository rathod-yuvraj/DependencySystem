using DependencySystem.DAL;
using DependencySystem.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetProjectDashboardAsync(int projectId)
        {
            var modules = await _context.Modules
                .Where(m => m.ProjectID == projectId)
                .Include(m => m.Tasks)
                .ToListAsync();

            var totalModules = modules.Count;
            var completedModules = modules.Count(m => m.Status == "Completed");
            var inProgressModules = modules.Count(m => m.Status == "InProgress");

            var tasks = modules.SelectMany(m => m.Tasks).ToList();
            var totalTasks = tasks.Count;
            var completedTasks = tasks.Count(t => t.Status == "Completed");
            var pendingTasks = tasks.Count(t => t.Status != "Completed");

            // Blocked tasks = tasks having dependencies
            var blockedTasks = await _context.TaskDependencies
                .CountAsync(td => td.Task.Module.ProjectID == projectId);

            double progress = totalTasks == 0
                ? 0
                : Math.Round((double)completedTasks / totalTasks * 100, 2);

            return new DashboardSummaryDto
            {
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalDepartments = await _context.Departments.CountAsync(),
                TotalProjects = await _context.Projects.CountAsync(),

                TotalModules = totalModules,
                CompletedModules = completedModules,
                InProgressModules = inProgressModules,

                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,

                ProjectProgressPercentage = progress,
                BlockedTasks = blockedTasks
            };
        }
    }
}
