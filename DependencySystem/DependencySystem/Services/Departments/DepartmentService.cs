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
        public async Task<List<DepartmentResponseDto>> GetByCompanyIdAsync(int companyId)
        {
            var companyExists = await _context.Companies
                .AnyAsync(c => c.CompanyID == companyId);

            if (!companyExists)
                throw new Exception("Company not found.");

            return await _context.Departments
                .Where(d => d.CompanyID == companyId)
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
        public async Task<DepartmentDashboardDto?> GetDepartmentDashboardAsync(int departmentId)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentID == departmentId);

            if (department == null) return null;

            // ===== Projects =====
            var projectQuery = _context.Projects
                .Where(p => p.DepartmentID == departmentId);

            var projectCount = await projectQuery.CountAsync();

            // ===== Modules =====
            var moduleQuery = _context.Modules
                .Where(m => m.Project.DepartmentID == departmentId);

            var moduleCount = await moduleQuery.CountAsync();

            // ===== Tasks =====
            var taskQuery = _context.Tasks
                .Where(t => t.Module.Project.DepartmentID == departmentId);

            var taskCount = await taskQuery.CountAsync();
            var completedTasks = await taskQuery.CountAsync(t => t.Status == Models.enums.TaskStatuss.Completed);
            var pendingTasks = taskCount - completedTasks;

            // ===== Developers =====
            var developerCount = await _context.ProjectTeamMembers
                .Where(pt => pt.Project.DepartmentID == departmentId)
                .Select(pt => pt.UserID)
                .Distinct()
                .CountAsync();

            // ===== Task Status Chart =====
            var taskStatusChart = new List<ChartItemDto>
    {
        new() { Name = "Completed", Value = completedTasks },
        new() { Name = "Pending", Value = pendingTasks }
    };

            // ===== Developer Workload =====
            var developerWorkload = await _context.Tasks
    .Where(t => t.Module.Project.DepartmentID == departmentId && t.AssignedToUserId != null)
    .GroupBy(t => t.AssignedToUserId!)
    .Select(g => new ChartItemDto
    {
        Name = g.Key,
        Value = g.Count()
    })
    .ToListAsync();


            // ===== Project Status (simple example) =====
            var projectStatusChart = new List<ChartItemDto>
    {
        new() { Name = "Total Projects", Value = projectCount }
    };

            return new DepartmentDashboardDto
            {
                DepartmentName = department.DepartmentName,

                ProjectCount = projectCount,
                ModuleCount = moduleCount,
                TaskCount = taskCount,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks,
                DeveloperCount = developerCount,

                ProjectStatusChart = projectStatusChart,
                TaskStatusChart = taskStatusChart,
                DeveloperWorkloadChart = developerWorkload
            };
        }

    }
}
