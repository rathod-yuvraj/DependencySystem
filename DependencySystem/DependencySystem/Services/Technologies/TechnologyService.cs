using DependencySystem.DAL;
using DependencySystem.DTOs.Technology;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Technologies
{
    public class TechnologyService : ITechnologyService
    {
        private readonly ApplicationDbContext _context;

        public TechnologyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Technology>> GetAllAsync()
            => await _context.Technologies.ToListAsync();

        public async Task<Technology> CreateAsync(TechnologyCreateDto dto)
        {
            var exists = await _context.Technologies
                .AnyAsync(t => t.TechnologyName == dto.TechnologyName);

            if (exists) throw new Exception("Technology already exists.");

            var tech = new Technology { TechnologyName = dto.TechnologyName };
            _context.Technologies.Add(tech);
            await _context.SaveChangesAsync();
            return tech;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tech = await _context.Technologies.FindAsync(id);
            if (tech == null) return false;

            _context.Technologies.Remove(tech);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> AssignToProjectAsync(int projectId, int techId)
        {
            if (!await _context.Projects.AnyAsync(p => p.ProjectID == projectId))
                throw new Exception("Project not found.");

            if (!await _context.Technologies.AnyAsync(t => t.TechnologyID == techId))
                throw new Exception("Technology not found.");

            var exists = await _context.ProjectTechnologies
                .AnyAsync(x => x.ProjectID == projectId && x.TechnologyID == techId);

            if (exists) return "Already assigned.";

            _context.ProjectTechnologies.Add(new ProjectTechnology
            {
                ProjectID = projectId,
                TechnologyID = techId
            });

            await _context.SaveChangesAsync();
            return "Assigned to project.";
        }

        public async Task<string> AssignToModuleAsync(int moduleId, int techId)
        {
            if (!await _context.Modules.AnyAsync(m => m.ModuleID == moduleId))
                throw new Exception("Module not found.");

            if (!await _context.Technologies.AnyAsync(t => t.TechnologyID == techId))
                throw new Exception("Technology not found.");

            var exists = await _context.ModuleTechnologies
                .AnyAsync(x => x.ModuleID == moduleId && x.TechnologyID == techId);

            if (exists) return "Already assigned.";

            _context.ModuleTechnologies.Add(new ModuleTechnology
            {
                ModuleID = moduleId,
                TechnologyID = techId
            });

            await _context.SaveChangesAsync();
            return "Assigned to module.";
        }

        public async Task<string> AssignToUserAsync(string userId, int techId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId))
                throw new Exception("User not found.");

            if (!await _context.Technologies.AnyAsync(t => t.TechnologyID == techId))
                throw new Exception("Technology not found.");

            var exists = await _context.UserTechnologies
                .AnyAsync(x => x.UserID == userId && x.TechnologyID == techId);

            if (exists) return "Already assigned.";

            _context.UserTechnologies.Add(new UserTechnology
            {
                UserID = userId,
                TechnologyID = techId
            });

            await _context.SaveChangesAsync();
            return "Assigned to user.";
        }
    }
}
