using DependencySystem.DTOs.Task;
using DependencySystem.Models;

namespace DependencySystem.Services.Tasks
{
    public interface ITaskService
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(int id);
        Task<List<TaskEntity>> GetByModuleIdAsync(int moduleId);
        Task<TaskEntity> CreateAsync(TaskCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
