using System.ComponentModel.DataAnnotations;

namespace DependencySystem.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditLogID { get; set; }

        public string UserID { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Details { get; set; } = string.Empty;
    }
}
