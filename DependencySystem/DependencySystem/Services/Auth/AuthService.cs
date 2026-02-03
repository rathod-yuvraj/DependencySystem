using DependencySystem.DAL;
using DependencySystem.DTOs.Auth;
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DependencySystem.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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

        public async Task<AuthResponseDto> SendOtpAsync(SendOtpRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "User not found with this email." };

            if (user.IsVerified)
                return new AuthResponseDto { Success = false, Message = "User already verified." };

            // Generate 6 digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Remove old OTP if exists
            var oldOtp = await _context.OtpVerifications
                .FirstOrDefaultAsync(x => x.Email == dto.Email && x.IsUsed == false);

            if (oldOtp != null)
                _context.OtpVerifications.Remove(oldOtp);

            var otpEntity = new OtpVerification
            {
                Email = dto.Email,
                OtpCode = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _context.OtpVerifications.AddAsync(otpEntity);
            await _context.SaveChangesAsync();

            // ✅ For now we return OTP in response (testing)
            // Later you will send OTP via Email/SMS.
            return new AuthResponseDto
            {
                Success = true,
                Message = $"OTP sent successfully. (TEST OTP: {otp})"
            };
        }

        public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "User not found." };

            var otpRecord = await _context.OtpVerifications
                .FirstOrDefaultAsync(x => x.Email == dto.Email && x.OtpCode == dto.OtpCode && x.IsUsed == false);

            if (otpRecord == null)
                return new AuthResponseDto { Success = false, Message = "Invalid OTP." };

            if (otpRecord.ExpiryTime < DateTime.UtcNow)
                return new AuthResponseDto { Success = false, Message = "OTP expired." };

            otpRecord.IsUsed = true;
            user.IsVerified = true;

            await _context.SaveChangesAsync();
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "OTP verified successfully. Account activated."
            };
        }
    }
}
