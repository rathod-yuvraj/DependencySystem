using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Task
{
    public class TaskCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public int ModuleID { get; set; }
    }
}
