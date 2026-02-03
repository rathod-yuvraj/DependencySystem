using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Auth
{
    public class VerifyOtpRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;
    }
}
