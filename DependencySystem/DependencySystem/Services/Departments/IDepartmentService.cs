using DependencySystem.DTOs.Department;
using DependencySystem.Models;

namespace DependencySystem.Services.Departments
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<List<Department>> GetByCompanyIdAsync(int companyId);
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(DepartmentCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
