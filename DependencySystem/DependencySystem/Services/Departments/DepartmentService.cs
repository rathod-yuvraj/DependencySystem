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

        public async Task<List<DepartmentResponseDto>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Company)
                .Include(d => d.Projects)
                .Select(d => new DepartmentResponseDto
                {
                    DepartmentID = d.DepartmentID,
                    DepartmentName = d.DepartmentName,
                    Company = new CompanyMiniDto
                    {
                        CompanyID = d.Company.CompanyID,
                        CompanyName = d.Company.CompanyName
                    },
                    Projects = d.Projects.Select(p => new ProjectMiniDto
                    {
                        ProjectID = p.ProjectID,
                        ProjectName = p.ProjectName
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<DepartmentResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Company)
                .Include(d => d.Projects)
                .Where(d => d.DepartmentID == id)
                .Select(d => new DepartmentResponseDto
                {
                    DepartmentID = d.DepartmentID,
                    DepartmentName = d.DepartmentName,
                    Company = new CompanyMiniDto
                    {
                        CompanyID = d.Company.CompanyID,
                        CompanyName = d.Company.CompanyName
                    },
                    Projects = d.Projects.Select(p => new ProjectMiniDto
                    {
                        ProjectID = p.ProjectID,
                        ProjectName = p.ProjectName
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto)
        {
            var exists = await _context.Departments
                .AnyAsync(d => d.CompanyID == dto.CompanyID && d.DepartmentName == dto.DepartmentName);

            if (exists)
                throw new Exception("Department already exists in this company.");

            var department = new Models.Department
            {
                DepartmentName = dto.DepartmentName,
                CompanyID = dto.CompanyID
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var company = await _context.Companies.FindAsync(dto.CompanyID);

            return new DepartmentResponseDto
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                Company = new CompanyMiniDto
                {
                    CompanyID = company!.CompanyID,
                    CompanyName = company.CompanyName
                },
                Projects = new List<ProjectMiniDto>()
            };
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
