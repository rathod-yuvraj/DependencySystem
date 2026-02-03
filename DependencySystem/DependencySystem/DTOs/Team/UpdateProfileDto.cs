using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Team
{
    public class UpdateProfileDto
    {
        public int? DepartmentID { get; set; }

        [Required]
        public string Designation { get; set; } = "Developer";

        public int ExperienceYears { get; set; } = 0;
    }
}
