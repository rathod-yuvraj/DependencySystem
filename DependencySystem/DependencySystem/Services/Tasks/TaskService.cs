using DependencySystem.DAL;
using DependencySystem.DTOs.Task;
using DependencySystem.Models;
using DependencySystem.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Module)
                .ThenInclude(m => m.Project)
                .ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Module)
                .FirstOrDefaultAsync(t => t.TaskID == id);
        }

        public async Task<List<TaskEntity>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.Tasks
                .Where(t => t.ModuleID == moduleId)
                .ToListAsync();
        }

        public async Task<TaskEntity> CreateAsync(TaskCreateDto dto)
        {
            var moduleExists = await _context.Modules.AnyAsync(m => m.ModuleID == dto.ModuleID);
            if (!moduleExists) throw new Exception("Module not found.");

            var task = new TaskEntity
            {
                Title = dto.Title,
                Description = dto.Description,
                ModuleID = dto.ModuleID,
                //Status = "Pending"
                Status=TaskStatuss.Pending
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
