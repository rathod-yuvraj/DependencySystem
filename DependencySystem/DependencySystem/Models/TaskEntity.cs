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
        public string Status { get; set; } = "Pending"; // Pending / InProgress / Completed

        [ForeignKey(nameof(Module))]
        public int ModuleID { get; set; }

        public Module? Module { get; set; }

        // ===================== NAVIGATIONS =====================

        // Task → Task Dependencies (this task depends on others)
        public ICollection<TaskDependency> TaskDependencies { get; set; }
            = new List<TaskDependency>();

        // Other tasks depending on this task
        public ICollection<TaskDependency> DependentTasks { get; set; }
            = new List<TaskDependency>();
    }
}
