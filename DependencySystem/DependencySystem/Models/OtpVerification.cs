using System.ComponentModel.DataAnnotations;

namespace DependencySystem.Models
{
    public class OtpVerification
    {
        [Key]
        public int OtpVerificationId { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}
