using DependencySystem.DTOs.Department;
using DependencySystem.Models;

namespace DependencySystem.Services.Departments
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDto>> GetAllAsync();
        Task<List<DepartmentResponseDto>> GetByCompanyIdAsync(int companyId);
        Task<DepartmentResponseDto?> GetByIdAsync(int id);
        Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<DepartmentDashboardDto?> GetDepartmentDashboardAsync(int departmentId);

    }
}
