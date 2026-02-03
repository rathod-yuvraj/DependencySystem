using DependencySystem.DAL;
using DependencySystem.DTOs.Department;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Departments
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Company)
                .ToListAsync();
        }

        public async Task<List<Department>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.Departments
                .Where(d => d.CompanyID == companyId)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);
        }

        public async Task<Department> CreateAsync(DepartmentCreateDto dto)
        {
            var companyExists = await _context.Companies.AnyAsync(c => c.CompanyID == dto.CompanyID);
            if (!companyExists)
                throw new Exception("Company not found.");

            var dept = new Department
            {
                DepartmentName = dto.DepartmentName,
                CompanyID = dto.CompanyID
            };

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();
            return dept;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            if (dept == null) return false;

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
