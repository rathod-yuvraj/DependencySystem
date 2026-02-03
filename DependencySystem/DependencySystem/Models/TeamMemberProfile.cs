using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class TeamMemberProfile
    {
        [Key]
        public int TeamMemberProfileID { get; set; }

        // FK to Identity User
        [Required]
        public string UserID { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        // Department (nullable)
        public int? DepartmentID { get; set; }
        public Department? Department { get; set; }

        public string Designation { get; set; } = "Developer";
        public int ExperienceYears { get; set; } = 0;
    }
}
