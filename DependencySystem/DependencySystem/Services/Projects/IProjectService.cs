using DependencySystem.DTOs.Project;
using DependencySystem.Models;

namespace DependencySystem.Services.Projects
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<ProjectTreeResponseDto?> GetProjectTreeAsync(int projectId);
        Task<List<Project>> GetByDepartmentIdAsync(int departmentId);
        Task<Project> CreateAsync(ProjectCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
