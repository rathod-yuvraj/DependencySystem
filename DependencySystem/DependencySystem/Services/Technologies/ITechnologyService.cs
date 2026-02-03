using DependencySystem.DTOs.Technology;
using DependencySystem.Models;

namespace DependencySystem.Services.Technologies
{
    public interface ITechnologyService
    {
        Task<List<Technology>> GetAllAsync();
        Task<Technology> CreateAsync(TechnologyCreateDto dto);
        Task<bool> DeleteAsync(int id);

        Task<string> AssignToProjectAsync(int projectId, int techId);
        Task<string> AssignToModuleAsync(int moduleId, int techId);
        Task<string> AssignToUserAsync(string userId, int techId);
    }
}
