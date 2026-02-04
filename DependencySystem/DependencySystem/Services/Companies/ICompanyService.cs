using DependencySystem.DTOs.Company;
using DependencySystem.Models;

namespace DependencySystem.Services.Companies
{
    public interface ICompanyService
    {
        Task<List<CompanyResponseDto>> GetAllAsync();
        Task<CompanyResponseDto?> GetByIdAsync(int id);
        Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
