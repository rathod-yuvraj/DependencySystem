using DependencySystem.Models;

namespace DependencySystem.Services.Audit
{
    public interface IAuditService
    {
        Task LogAsync(AuditLog log);
    }
}
