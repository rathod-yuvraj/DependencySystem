using System.ComponentModel.DataAnnotations;

namespace DependencySystem.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
