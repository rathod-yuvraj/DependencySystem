using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Team
{
    public class AssignProjectMemberDto
    {
        [Required]
        public int ProjectID { get; set; }

        [Required]
        public string UserID { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Developer"; // Manager/Developer/Maintainer
    }
}
