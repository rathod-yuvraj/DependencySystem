using DependencySystem.DTOs.Auth;
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;

namespace DependencySystem.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
                return new AuthResponseDto { Success = false, Message = "Email already exists." };

            var existingUserByUsername = await _userManager.FindByNameAsync(dto.Username);
            if (existingUserByUsername != null)
                return new AuthResponseDto { Success = false, Message = "Username already exists." };

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                IsVerified = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(" | ", result.Errors.Select(e => e.Description))
                };
            }

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully. Please verify OTP."
            };
        }
    }
}
