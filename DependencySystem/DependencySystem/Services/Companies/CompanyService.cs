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
        public async Task<CompanyDetailAnalyticsDto?> GetCompanyDetailAnalyticsAsync(int companyId)
        {
            var company = await _context.Companies
                .Where(c => c.CompanyID == companyId)
                .Select(c => new CompanyDetailAnalyticsDto
                {
                    CompanyName = c.CompanyName,

                    DepartmentCount = c.Departments.Count(),

                    ProjectCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .Count(),

                    ModuleCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .Count(),

                    TaskCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .SelectMany(m => m.Tasks)
                        .Count(),

                    DeveloperCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.ProjectTeamMembers)
                        .Select(ptm => ptm.UserID)
                        .Distinct()
                        .Count(),

                    // ===== Task Status Pie =====
                    TaskStatusChart = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .SelectMany(m => m.Tasks)
                        .GroupBy(t => t.Status)
                        .Select(g => new ChartItemDto
                        {
                            Name = g.Key.ToString(),
                            Value = g.Count()
                        })
                        .ToList(),

                    // ===== Projects per Department =====
                    ProjectsPerDepartmentChart = c.Departments
                        .Select(d => new ChartItemDto
                        {
                            Name = d.DepartmentName,
                            Value = d.Projects.Count()
                        })
                        .ToList(),

                    // ===== Developer workload =====
                    DeveloperWorkloadChart = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.ProjectTeamMembers)
                        .GroupBy(ptm => ptm.User.UserName)
                        .Select(g => new ChartItemDto
                        {
                            Name = g.Key!,
                            Value = g.Count()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return company;
        }
        public async Task<CompanyDashboardDto?> GetCompanyDashboardAsync(int companyId)
        {
            return await _context.Companies
                .Where(c => c.CompanyID == companyId)
                .Select(c => new CompanyDashboardDto
                {
                    CompanyName = c.CompanyName,

                    DepartmentCount = c.Departments.Count(),

                    ProjectCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .Count(),

                    ModuleCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .Count(),

                    TaskCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .SelectMany(m => m.Tasks)
                        .Count(),

                    DeveloperCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.ProjectTeamMembers)
                        .Select(ptm => ptm.UserID)
                        .Distinct()
                        .Count(),

                    // ===== Projects per Department =====
                    ProjectsPerDepartment = c.Departments
                        .Select(d => new ChartItemDto
                        {
                            Name = d.DepartmentName,
                            Value = d.Projects.Count()
                        })
                        .ToList(),

                    // ===== Task Status Pie =====
                    TaskStatus = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .SelectMany(m => m.Tasks)
                        .GroupBy(t => t.Status)
                        .Select(g => new ChartItemDto
                        {
                            Name = g.Key.ToString(),
                            Value = g.Count()
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<CompanyAnalyticsDto>> GetCompanyAnalyticsAsync()
        {
            return await _context.Companies
                .Select(c => new CompanyAnalyticsDto
                {
                    CompanyID = c.CompanyID,
                    CompanyName = c.CompanyName,

                    DepartmentCount = c.Departments.Count(),

                    ProjectCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .Count(),

                    ModuleCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .Count(),

                    TaskCount = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.Modules)
                        .SelectMany(m => m.Tasks)
                        .Count(),

                    Developers = c.Departments
                        .SelectMany(d => d.Projects)
                        .SelectMany(p => p.ProjectTeamMembers)
                        .Select(ptm => ptm.User.UserName!)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();
        }

    }

}
