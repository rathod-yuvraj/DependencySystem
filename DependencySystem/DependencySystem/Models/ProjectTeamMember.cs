using System.ComponentModel.DataAnnotations.Schema;

namespace DependencySystem.Models
{
    public class ProjectTeamMember
    {
        public int ProjectID { get; set; }
        public Project? Project { get; set; }

        public string UserID { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public string Role { get; set; } = "Developer"; // Manager/Developer/Maintainer
    }
}
