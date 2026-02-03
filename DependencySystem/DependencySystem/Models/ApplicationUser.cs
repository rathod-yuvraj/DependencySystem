using Microsoft.AspNetCore.Identity;

namespace DependencySystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsVerified { get; set; } = false;

        public TeamMemberProfile? TeamMemberProfile { get; set; }
        public ICollection<ProjectTeamMember> ProjectTeamMembers { get; set; } = new List<ProjectTeamMember>();

    }
}
