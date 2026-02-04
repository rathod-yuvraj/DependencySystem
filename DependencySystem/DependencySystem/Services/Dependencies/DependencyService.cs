using DependencySystem.DAL;
using DependencySystem.DTOs.Dependency;
using DependencySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Dependencies
{
    public class DependencyService : IDependencyService
    {
        private readonly ApplicationDbContext _context;

        public DependencyService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================
        // MODULE DEPENDENCIES
        // ============================

        public async Task<List<Dependency>> GetModuleDependenciesAsync(int moduleId)
        {
            return await _context.Dependencies
                .Where(d => d.SourceModuleID == moduleId || d.TargetModuleID == moduleId)
                .Include(d => d.SourceModule)
                .Include(d => d.TargetModule)
                .ToListAsync();
        }

        public async Task<Dependency> AddModuleDependencyAsync(AddModuleDependencyDto dto)
        {
            if (dto.SourceModuleID == dto.TargetModuleID)
                throw new Exception("Module cannot depend on itself.");

            var sourceExists = await _context.Modules.AnyAsync(m => m.ModuleID == dto.SourceModuleID);
            var targetExists = await _context.Modules.AnyAsync(m => m.ModuleID == dto.TargetModuleID);

            if (!sourceExists || !targetExists)
                throw new Exception("Source or Target Module not found.");

            var alreadyExists = await _context.Dependencies.AnyAsync(d =>
                d.SourceModuleID == dto.SourceModuleID && d.TargetModuleID == dto.TargetModuleID);

            if (alreadyExists)
                throw new Exception("Dependency already exists.");

            var dependency = new Dependency
            {
                SourceModuleID = dto.SourceModuleID,
                TargetModuleID = dto.TargetModuleID
            };

            _context.Dependencies.Add(dependency);
            await _context.SaveChangesAsync();

            return dependency;
        }

        public async Task<bool> RemoveModuleDependencyAsync(int dependencyId)
        {
            var dep = await _context.Dependencies.FindAsync(dependencyId);
            if (dep == null) return false;

            _context.Dependencies.Remove(dep);
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // TASK DEPENDENCIES
        // ============================

        public async Task<List<TaskDependency>> GetTaskDependenciesAsync(int taskId)
        {
            return await _context.TaskDependencies
                .Where(td => td.TaskID == taskId)
                .Include(td => td.Task)
                .Include(td => td.DependsOnTask)
                .ToListAsync();
        }

        public async Task<TaskDependency> AddTaskDependencyAsync(AddTaskDependencyDto dto)
        {
            if (dto.TaskID == dto.DependsOnTaskID)
                throw new Exception("Task cannot depend on itself.");

            var taskExists = await _context.Tasks.AnyAsync(t => t.TaskID == dto.TaskID);
            var dependsExists = await _context.Tasks.AnyAsync(t => t.TaskID == dto.DependsOnTaskID);

            if (!taskExists || !dependsExists)
                throw new Exception("Task or dependent task not found.");

            var alreadyExists = await _context.TaskDependencies.AnyAsync(td =>
                td.TaskID == dto.TaskID && td.DependsOnTaskID == dto.DependsOnTaskID);

            if (alreadyExists)
                throw new Exception("Task dependency already exists.");

            var tdEntity = new TaskDependency
            {
                TaskID = dto.TaskID,
                DependsOnTaskID = dto.DependsOnTaskID
            };

            _context.TaskDependencies.Add(tdEntity);
            await _context.SaveChangesAsync();

            return tdEntity;
        }

        public async Task<bool> RemoveTaskDependencyAsync(int taskId, int dependsOnTaskId)
        {
            var td = await _context.TaskDependencies
                .FirstOrDefaultAsync(x => x.TaskID == taskId && x.DependsOnTaskID == dependsOnTaskId);

            if (td == null) return false;

            _context.TaskDependencies.Remove(td);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<DependencyGraphDto> GetProjectDependencyGraphAsync(int projectId)
        {
            var graph = new DependencyGraphDto();

            var modules = await _context.Modules
                .Where(m => m.ProjectID == projectId)
                .Include(m => m.Tasks)
                .ToListAsync();

            // MODULE NODES
            foreach (var m in modules)
            {
                graph.Nodes.Add(new NodeDto
                {
                    Id = $"M{m.ModuleID}",
                    Label = m.ModuleName,
                    Type = "Module",
                    Status = m.Status
                });
            }

            // TASK NODES
            foreach (var m in modules)
            {
                foreach (var t in m.Tasks)
                {
                    graph.Nodes.Add(new NodeDto
                    {
                        Id = $"T{t.TaskID}",
                        Label = t.Title,
                        Type = "Task",
                        Status = t.Status
                    });

                    // Module → Task edge
                    graph.Edges.Add(new EdgeDto
                    {
                        Source = $"M{m.ModuleID}",
                        Target = $"T{t.TaskID}",
                        Relation = "contains"
                    });
                }
            }

            // MODULE DEPENDENCIES
            var moduleDeps = await _context.Dependencies
                .Include(d => d.SourceModule)
                .Include(d => d.TargetModule)
                .Where(d => d.SourceModule.ProjectID == projectId)
                .ToListAsync();

            foreach (var d in moduleDeps)
            {
                graph.Edges.Add(new EdgeDto
                {
                    Source = $"M{d.SourceModuleID}",
                    Target = $"M{d.TargetModuleID}",
                    Relation = "depends_on"
                });
            }

            // TASK DEPENDENCIES
            var taskDeps = await _context.TaskDependencies
                .Include(td => td.Task)
                .Include(td => td.DependsOnTask)
                .Where(td => td.Task.Module.ProjectID == projectId)
                .ToListAsync();

            foreach (var td in taskDeps)
            {
                graph.Edges.Add(new EdgeDto
                {
                    Source = $"T{td.TaskID}",
                    Target = $"T{td.DependsOnTaskID}",
                    Relation = "depends_on"
                });
            }

            return graph;
        }
    }
}
