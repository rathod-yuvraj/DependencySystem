using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class TaskEntity
    {
        [Key]
        public int TaskID { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending"; // Pending/InProgress/Completed

        [ForeignKey("Module")]
        public int ModuleID { get; set; }

        public Module? Module { get; set; }
    }
}
