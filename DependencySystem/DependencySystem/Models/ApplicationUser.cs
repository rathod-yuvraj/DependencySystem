using Microsoft.AspNetCore.Identity;

namespace DependencySystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsVerified { get; set; } = false;
    }
}
