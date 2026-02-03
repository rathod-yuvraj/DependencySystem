using DependencySystem.DAL;
using DependencySystem.DTOs.Company;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Company>> GetAllAsync()
            => await _context.Companies.Include(x => x.Departments).ToListAsync();

        public async Task<Company?> GetByIdAsync(int id)
            => await _context.Companies.Include(x => x.Departments)
                    .FirstOrDefaultAsync(x => x.CompanyID == id);

        public async Task<Company> CreateAsync(CompanyCreateDto dto)
        {
            var company = new Company { CompanyName = dto.CompanyName };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null) return false;

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
