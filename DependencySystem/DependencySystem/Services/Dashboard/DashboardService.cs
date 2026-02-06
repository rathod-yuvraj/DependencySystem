using DependencySystem.DAL;
using DependencySystem.DTOs.Company;
using DependencySystem.DTOs.Dashboard;
using DependencySystem.Models.enums;
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

        public async Task<ManagerDashboardDto?> GetManagerDashboardAsync(int projectId)
        {
            return await _context.Projects
                .Where(p => p.ProjectID == projectId)
                .Select(p => new ManagerDashboardDto
                {
                    ProjectName = p.ProjectName,

                    ModuleCount = p.Modules.Count(),

                    TaskCount = p.Modules
                        .SelectMany(m => m.Tasks)
                        .Count(),

                    CompletedTasks = p.Modules
                        .SelectMany(m => m.Tasks)
                        .Count(t => t.Status == TaskStatuss.Completed),

                    PendingTasks = p.Modules
                        .SelectMany(m => m.Tasks)
                        .Count(t => t.Status != TaskStatuss.Completed),

                    BlockedTasks = p.Modules
                        .SelectMany(m => m.Tasks)
                        .Count(t => t.TaskDependencies.Any()),

                    DeveloperCount = p.ProjectTeamMembers
                        .Select(ptm => ptm.UserID)
                        .Distinct()
                        .Count(),

                    // ===== Task Status Pie =====
                    TaskStatusChart = p.Modules
                        .SelectMany(m => m.Tasks)
                        .GroupBy(t => t.Status)
                        .Select(g => new ChartItemDto
                        {
                            Name = g.Key.ToString(),
                            Value = g.Count()
                        })
                        .ToList(),

                    // ===== Module Progress Bar =====
                    ModuleProgressChart = p.Modules
                        .Select(m => new ChartItemDto
                        {
                            Name = m.ModuleName,
                            Value = m.Tasks.Count(t => t.Status == TaskStatuss.Completed)
                        })
                        .ToList(),

                    // ===== Developer Workload =====
                    DeveloperWorkloadChart = p.ProjectTeamMembers
                        .GroupBy(ptm => ptm.User.UserName)
                        .Select(g => new ChartItemDto
                        {
                            Name = g.Key!,
                            Value = g.Count()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DashboardSummaryDto> GetProjectDashboardAsync(int projectId)
        {
            // ---------- ORGANIZATION STATS ----------
            var organizationStats = await Task.WhenAll(
                _context.Companies.CountAsync(),
                _context.Departments.CountAsync(),
                _context.Projects.CountAsync()
            );

            // ---------- MODULE STATS ----------
            var moduleQuery = _context.Modules.Where(m => m.ProjectID == projectId);

            var totalModules = await moduleQuery.CountAsync();
            var completedModules = await moduleQuery.CountAsync(m => m.Status == ModuleStatus.Completed);
            var inProgressModules = await moduleQuery.CountAsync(m => m.Status == ModuleStatus.InProgress);

            // ---------- TASK STATS ----------
            var taskQuery = _context.Tasks
                .Where(t => t.Module.ProjectID == projectId);

            var totalTasks = await taskQuery.CountAsync();
            var completedTasks = await taskQuery.CountAsync(t => t.Status == TaskStatuss.Completed);
            var pendingTasks = await taskQuery.CountAsync(t => t.Status != TaskStatuss.Completed);

            // ---------- BLOCKED TASKS ----------
            var blockedTasks = await _context.TaskDependencies
                .Where(td => td.Task.Module.ProjectID == projectId)
                .Select(td => td.TaskID)
                .Distinct()
                .CountAsync();

            // ---------- PROGRESS ----------
            double progress = totalTasks == 0
                ? 0
                : Math.Round((double)completedTasks / totalTasks * 100, 2);

            // ---------- DTO ----------
            return new DashboardSummaryDto
            {
                Organization = new OrganizationStats
                {
                    TotalCompanies = organizationStats[0],
                    TotalDepartments = organizationStats[1],
                    TotalProjects = organizationStats[2]
                },

                Modules = new ModuleStats
                {
                    TotalModules = totalModules,
                    CompletedModules = completedModules,
                    InProgressModules = inProgressModules
                },

                Tasks = new TaskStats
                {
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    PendingTasks = pendingTasks
                },

                ProjectProgressPercentage = progress,
                BlockedTasks = blockedTasks
            };
        }
    }
}
