using DependencySystem.DAL;
using DependencySystem.DTOs.Task;
using DependencySystem.Models;
using DependencySystem.Models.enums;
using DependencySystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ProjectHub> _hub;

        public TaskService(ApplicationDbContext context, IHubContext<ProjectHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        /* ================= GET ALL ================= */

        public async Task<List<TaskResponseDto>> GetAllAsync()
        {
            return await _context.Tasks
                .Select(t => new TaskResponseDto
                {
                    TaskID = t.TaskID,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    ModuleID = t.ModuleID
                })
                .ToListAsync();
        }

        /* ================= GET BY ID ================= */

        public async Task<TaskResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Where(t => t.TaskID == id)
                .Select(t => new TaskResponseDto
                {
                    TaskID = t.TaskID,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    ModuleID = t.ModuleID
                })
                .FirstOrDefaultAsync();
        }

        /* ================= GET BY MODULE ================= */

        public async Task<List<TaskResponseDto>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.Tasks
                .Where(t => t.ModuleID == moduleId)
                .Select(t => new TaskResponseDto
                {
                    TaskID = t.TaskID,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    ModuleID = t.ModuleID
                })
                .ToListAsync();
        }

        /* ================= CREATE ================= */

        public async Task<TaskResponseDto> CreateAsync(TaskCreateDto dto)
        {
            var module = await _context.Modules
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.ModuleID == dto.ModuleID);

            if (module == null)
                throw new Exception("Module not found.");

            var task = new TaskEntity
            {
                Title = dto.Title,
                Description = dto.Description,
                ModuleID = dto.ModuleID,
                Status = TaskStatuss.Pending
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            /* 🔴 REAL-TIME EVENT */
            await _hub.Clients
                .Group($"project-{module.ProjectID}")
                .SendAsync("TaskCreated", new
                {
                    task.TaskID,
                    task.Title,
                    Status = task.Status.ToString(),
                    task.ModuleID
                });

            return new TaskResponseDto
            {
                TaskID = task.TaskID,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                ModuleID = task.ModuleID
            };
        }

        /* ================= UPDATE ================= */

        public async Task<TaskResponseDto?> UpdateAsync(int id, TaskUpdateDto dto)
        {
            var task = await _context.Tasks
                .Include(t => t.Module)
                .ThenInclude(m => m.Project)
                .FirstOrDefaultAsync(t => t.TaskID == id);

            if (task == null) return null;

            task.Title = dto.Title;
            task.ModuleID = dto.ModuleID;
            task.Status = Enum.Parse<TaskStatuss>(dto.Status);

            await _context.SaveChangesAsync();

            /* 🔴 REAL-TIME EVENT */
            await _hub.Clients
                .Group($"project-{task.Module.ProjectID}")
                .SendAsync("TaskUpdated", new
                {
                    task.TaskID,
                    task.Title,
                    Status = task.Status.ToString(),
                    task.ModuleID
                });

            return new TaskResponseDto
            {
                TaskID = task.TaskID,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                ModuleID = task.ModuleID
            };
        }

        /* ================= DELETE ================= */

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Module)
                .ThenInclude(m => m.Project)
                .FirstOrDefaultAsync(t => t.TaskID == id);

            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            /* 🔴 REAL-TIME EVENT */
            await _hub.Clients
                .Group($"project-{task.Module.ProjectID}")
                .SendAsync("TaskDeleted", id);

            return true;
        }
    }
}
