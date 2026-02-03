using DependencySystem.DTOs.Company;
using DependencySystem.Models;

namespace DependencySystem.Services.Companies
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(int id);
        Task<Company> CreateAsync(CompanyCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
