using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Dependency
{
    public class AddTaskDependencyDto
    {
        [Required]
        public int TaskID { get; set; }

        [Required]
        public int DependsOnTaskID { get; set; }
    }
}
