using DependencySystem.DTOs.Module;
using DependencySystem.Models;

namespace DependencySystem.Services.Modules
{
    public interface IModuleService
    {
        Task<List<Module>> GetAllAsync();
        Task<Module?> GetByIdAsync(int id);
        Task<List<Module>> GetByProjectIdAsync(int projectId);
        Task<Module> CreateAsync(ModuleCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
