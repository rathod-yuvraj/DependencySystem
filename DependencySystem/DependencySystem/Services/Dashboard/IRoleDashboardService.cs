using DependencySystem.DTOs.Dashboard;

namespace DependencySystem.Services.Dashboard
{
    public interface IRoleDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<ManagerDashboardDto> GetManagerDashboardAsync(int projectId);
        Task<DeveloperDashboardDto> GetDeveloperDashboardAsync(string userId);
    }
}
