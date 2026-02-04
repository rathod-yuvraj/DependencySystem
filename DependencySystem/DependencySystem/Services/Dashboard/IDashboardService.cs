using DependencySystem.DTOs.Dashboard;

namespace DependencySystem.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetProjectDashboardAsync(int projectId);
    }
}
