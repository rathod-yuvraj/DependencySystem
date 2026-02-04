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

        public async Task<List<CompanyResponseDto>> GetAllAsync()
        {
             return await _context.Companies.Include(c => c.Departments).Select(c=>new CompanyResponseDto
             {
                 CompanyID = c.CompanyID,
                 CompanyName = c.CompanyName,
                 Departments = c.Departments.Select(d => new DepartmentSimpleDto
                 {
                     DepartmentID = d.DepartmentID,
                     DepartmentName = d.DepartmentName,
                     CompanyID = d.CompanyID
                 }).ToList()

             } ).ToListAsync();

        }


        public async Task<CompanyResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.Departments)
                .Where(c => c.CompanyID == id)
                .Select(c => new CompanyResponseDto
                {
                    CompanyID = c.CompanyID,
                    CompanyName = c.CompanyName,
                    Departments = c.Departments.Select(d => new DepartmentSimpleDto
                    {
                        DepartmentID = d.DepartmentID,
                        DepartmentName = d.DepartmentName,
                        CompanyID = d.CompanyID
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
        public async Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto)
        {
            var exists = await _context.Companies
                .AnyAsync(c => c.CompanyName == dto.CompanyName);

            if (exists)
                throw new Exception("Company already exists.");

            var company = new Models.Company
            {
                CompanyName = dto.CompanyName
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return new CompanyResponseDto
            {
                CompanyID = company.CompanyID,
                CompanyName = company.CompanyName,
                Departments = new List<DepartmentSimpleDto>()
            };
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
