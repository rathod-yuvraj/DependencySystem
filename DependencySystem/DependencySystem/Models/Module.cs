using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class Module
    {
        [Key]
        public int ModuleID { get; set; }

        [Required, MaxLength(200)]
        public string ModuleName { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending"; // Pending/InProgress/Completed

        [ForeignKey(nameof(Project))]
        public int ProjectID { get; set; }

        public Project? Project { get; set; }

        // ===================== NAVIGATIONS =====================

        // Module → Tasks
        public ICollection<TaskEntity> Tasks { get; set; }
            = new List<TaskEntity>();

        // Dependency graph
        public ICollection<Dependency> OutgoingDependencies { get; set; }
            = new List<Dependency>();

        public ICollection<Dependency> IncomingDependencies { get; set; }
            = new List<Dependency>();

        // Module → Technologies
        public ICollection<ModuleTechnology> ModuleTechnologies { get; set; }
            = new List<ModuleTechnology>();
    }
}
