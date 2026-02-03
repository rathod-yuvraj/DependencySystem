using System.ComponentModel.DataAnnotations;

namespace DependencySystem.Models
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; } = false;

        // FK
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
