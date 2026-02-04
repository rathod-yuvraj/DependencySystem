using DependencySystem.DAL;
using DependencySystem.DTOs.Auth;
using DependencySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DependencySystem.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
            _emailService = emailService;
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

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(dto.EmailOrUsername);

            if (user == null)
                user = await _userManager.FindByNameAsync(dto.EmailOrUsername);

            if (user == null)
                return new LoginResponseDto { Success = false, Message = "Invalid credentials." };

            if (!user.IsVerified)
                return new LoginResponseDto { Success = false, Message = "User not verified. Please verify OTP." };

            var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!validPassword)
                return new LoginResponseDto { Success = false, Message = "Invalid credentials." };

            // ✅ Create JWT
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Name, user.UserName ?? "")
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSection["DurationInMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // ✅ Refresh Token
            var refreshToken = Guid.NewGuid().ToString();
            var refreshEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshEntity);
            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful.",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var savedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken && x.IsRevoked == false);

            if (savedToken == null)
                return new LoginResponseDto { Success = false, Message = "Invalid refresh token." };

            if (savedToken.ExpiryDate < DateTime.UtcNow)
                return new LoginResponseDto { Success = false, Message = "Refresh token expired." };

            var user = await _userManager.FindByIdAsync(savedToken.UserId);
            if (user == null)
                return new LoginResponseDto { Success = false, Message = "User not found." };

            // ✅ revoke old refresh token (rotation)
            savedToken.IsRevoked = true;

            // ✅ create new access token
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Name, user.UserName ?? "")
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSection["DurationInMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // ✅ generate new refresh token
            var newRefreshToken = Guid.NewGuid().ToString();
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully.",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Expiration = token.ValidTo
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
            await _userManager.AddToRoleAsync(user, Helper.AppRoles.Developer);

            await _context.SaveChangesAsync();
            await _userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                Success = true,
                Message = "OTP verified successfully. Account activated."
            };
        }

        public async Task<AuthResponseDto> SendOtpAsync(SendOtpRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResponseDto { Success = false, Message = "User not found with this email." };

            if (user.IsVerified)
                return new AuthResponseDto { Success = false, Message = "User already verified." };

            var otp = new Random().Next(100000, 999999).ToString();

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

            // ✅ Send OTP Email
            var subject = "OTP Verification - DependencySystem";
            var body = $@"
        <h3>Your OTP Code</h3>
        <p>Your OTP is: <b style='font-size:20px'>{otp}</b></p>
        <p>This OTP will expire in 5 minutes.</p>
    ";

            await _emailService.SendEmailAsync(dto.Email, subject, body);

            return new AuthResponseDto
            {
                Success = true,
                Message = "OTP sent successfully to your email."
            };
        }

    }
}