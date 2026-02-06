using DependencySystem.Models;

namespace DependencySystem.Services.Audit
{
    public interface IAuditService
    {
        //Task AuditLogAsyncs(AuditLog log);
        Task LogAsync(string action, string entityType, int entityId,
                  string description, string userId, int projectId);
    }
}
