using DependencySystem.DAL;
using DependencySystem.DTOs.Module;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Modules
{
    public class ModuleService : IModuleService
    {
        private readonly ApplicationDbContext _context;

        public ModuleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Module>> GetAllAsync()
        {
            return await _context.Modules
                .Include(m => m.Project)
                .ThenInclude(p => p.Department)
                .ToListAsync();
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            return await _context.Modules
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.ModuleID == id);
        }

        public async Task<List<Module>> GetByProjectIdAsync(int projectId)
        {
            return await _context.Modules
                .Where(m => m.ProjectID == projectId)
                .ToListAsync();
        }

        public async Task<Module> CreateAsync(ModuleCreateDto dto)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectID == dto.ProjectID);
            if (!projectExists) throw new Exception("Project not found.");

            var module = new Module
            {
                ModuleName = dto.ModuleName,
                ProjectID = dto.ProjectID,
                Status = "Pending"
            };

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return false;

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
