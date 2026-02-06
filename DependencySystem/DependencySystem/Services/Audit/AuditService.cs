//using DependencySystem.DAL;
//using DependencySystem.Models;
//using System.Threading.Tasks;

//namespace DependencySystem.Services.Audit
//{
//    public class AuditService : IAuditService
//    {
//        private readonly ApplicationDbContext _context;

//        public AuditService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task LogAsync(string action, string entityType, int entityId,
//                                string description, string userId, int projectId)
//        {
//            _context.AuditLogs.Add(new AuditLog
//            {
//                Action = action,
//                EntityType = entityType,
//                EntityId = entityId,
//                Description = description,
//                UserId = userId,
//                ProjectId = projectId,
//                CreatedAt = DateTime.UtcNow
//            });
//            await _audit.LogAsync(
//    "StatusChanged",
//    "Task",
//    task.TaskID,
//    $"Task '{task.Title}' moved to {task.Status}",
//    userId,
//    task.Module.ProjectID
//);

//            await _context.SaveChangesAsync();
//        }
//    }
//}
