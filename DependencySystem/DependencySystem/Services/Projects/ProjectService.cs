using DependencySystem.DAL;
using DependencySystem.DTOs.Project;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Department)
                .ThenInclude(d => d.Company)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Department)
                .ThenInclude(d => d.Company)
                .FirstOrDefaultAsync(p => p.ProjectID == id);
        }

        public async Task<List<Project>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Projects
                .Where(p => p.DepartmentID == departmentId)
                .ToListAsync();
        }

        public async Task<Project> CreateAsync(ProjectCreateDto dto)
        {
            var deptExists = await _context.Departments.AnyAsync(d => d.DepartmentID == dto.DepartmentID);
            if (!deptExists) throw new Exception("Department not found.");

            var project = new Project
            {
                ProjectName = dto.ProjectName,
                DepartmentID = dto.DepartmentID
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
