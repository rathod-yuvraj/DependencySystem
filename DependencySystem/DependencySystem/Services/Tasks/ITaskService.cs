using DependencySystem.DTOs.Task;
using DependencySystem.Models;

namespace DependencySystem.Services.Tasks
{
    public interface ITaskService
    {

        
        Task<List<TaskResponseDto>> GetByModuleIdAsync(int moduleId);

        
        Task<List<TaskResponseDto>> GetAllAsync();
        Task<TaskResponseDto?> GetByIdAsync(int id);
       
        Task<TaskResponseDto> CreateAsync(TaskCreateDto dto);
        Task<TaskResponseDto?> UpdateAsync(int id, TaskUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
