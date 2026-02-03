using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Project
{
    public class ProjectCreateDto
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        public int DepartmentID { get; set; }
    }
}
