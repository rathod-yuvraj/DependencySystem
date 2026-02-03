using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Auth
{
    public class SendOtpRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
