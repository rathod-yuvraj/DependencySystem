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

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        public Project? Project { get; set; }
    }
}
