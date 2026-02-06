using DependencySystem.DAL;
using DependencySystem.DTOs.Module;
using DependencySystem.Hubs;
using DependencySystem.Models;
using DependencySystem.Models.enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Modules
{
    public class ModuleService : IModuleService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ProjectHub> _hub;

        public ModuleService(ApplicationDbContext context, IHubContext<ProjectHub> hub)
        {
            _context = context;
            _hub = hub;
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
                //Status = "Pending"
                Status=Models.enums.ModuleStatus.Pending
            };
            await _context.SaveChangesAsync();

            await _hub.Clients
                .Group($"project-{module.ProjectID}")
                .SendAsync("ModuleCreated", module);

            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return false;
            await _hub.Clients
    .Group($"project-{module.ProjectID}")
    .SendAsync("ModuleDeleted", id);

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Module?> UpdateAsync(int id, ModuleUpdateDto dto)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return null;

            var projectExists = await _context.Projects
                .AnyAsync(p => p.ProjectID == dto.ProjectID);

            if (!projectExists)
                throw new Exception("Project not found.");
            await _hub.Clients
    .Group($"project-{module.ProjectID}")
    .SendAsync("ModuleUpdated", module);

            module.ModuleName = dto.ModuleName;
            module.ProjectID = dto.ProjectID;
            module.Status = Enum.Parse<ModuleStatus>(dto.Status);

            await _context.SaveChangesAsync();
            return module;
        }

    }
}
