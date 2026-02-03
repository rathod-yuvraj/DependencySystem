using DependencySystem.DTOs.Dependency;
using DependencySystem.Models;

namespace DependencySystem.Services.Dependencies
{
    public interface IDependencyService
    {
        // Module dependencies
        Task<List<Dependency>> GetModuleDependenciesAsync(int moduleId);
        Task<Dependency> AddModuleDependencyAsync(AddModuleDependencyDto dto);
        Task<bool> RemoveModuleDependencyAsync(int dependencyId);

        // Task dependencies
        Task<List<TaskDependency>> GetTaskDependenciesAsync(int taskId);
        Task<TaskDependency> AddTaskDependencyAsync(AddTaskDependencyDto dto);
        Task<bool> RemoveTaskDependencyAsync(int taskId, int dependsOnTaskId);
    }
}
