using DependencySystem.DAL;
using DependencySystem.DTOs.Auth;
using DependencySystem.Helper;
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

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration config,
            IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        // =====================================================
        // REGISTER
        // =====================================================
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return Fail("Email already exists.");

            if (await _userManager.FindByNameAsync(dto.Username) != null)
                return Fail("Username already exists.");

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
      
                IsVerified = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return Fail(string.Join(" | ", result.Errors.Select(e => e.Description)));

            return Success("User registered successfully. Please verify OTP.");
        }

        // =====================================================
        // LOGIN
        // =====================================================
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.EmailOrUsername)
                       ?? await _userManager.FindByNameAsync(dto.EmailOrUsername);

            if (user == null)
                return new LoginResponseDto { Success = false, Message = "Invalid credentials." };

            if (!user.IsVerified)
                return new LoginResponseDto { Success = false, Message = "User not verified." };

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
                return new LoginResponseDto { Success = false, Message = "Invalid credentials." };

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
            var key = GetJwtKey();

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSection["DurationInMinutes"])
                ),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }

        // =====================================================
        // REFRESH TOKEN
        // =====================================================
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == dto.RefreshToken && !x.IsRevoked);

            if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
                return new LoginResponseDto { Success = false, Message = "Invalid refresh token." };

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return new LoginResponseDto { Success = false, Message = "User not found." };

            storedToken.IsRevoked = true;

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
            var key = GetJwtKey();

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(jwtSection["DurationInMinutes"])
                ),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

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
                Message = "Token refreshed",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Expiration = token.ValidTo
            };
        }


        // =====================================================
        // OTP VERIFICATION
        // =====================================================
        public async Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Fail("User not found.");

            var otp = await _context.OtpVerifications.FirstOrDefaultAsync(x =>
                x.Email == dto.Email &&
                x.OtpCode == dto.OtpCode &&
                !x.IsUsed &&
                x.ExpiryTime > DateTime.UtcNow);

            if (otp == null)
                return Fail("Invalid or expired OTP.");

            otp.IsUsed = true;
            user.IsVerified = true;

            await _userManager.AddToRoleAsync(user, AppRoles.Developer);

            await _context.SaveChangesAsync();
            await _userManager.UpdateAsync(user);

            return Success("OTP verified successfully. Account activated.");
        }

        // =====================================================
        // SEND OTP
        // =====================================================
        public async Task<AuthResponseDto> SendOtpAsync(SendOtpRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Fail("User not found.");

            if (user.IsVerified)
                return Fail("User already verified.");

            await InvalidateOldOtp(dto.Email);

            var otp = GenerateOtp();

            await _context.OtpVerifications.AddAsync(new OtpVerification
            {
                Email = dto.Email,
                OtpCode = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                dto.Email,
                "OTP Verification - DependencySystem",
                $"<h3>Your OTP</h3><p><b>{otp}</b> (valid for 5 minutes)</p>"
            );

            return Success("OTP sent successfully.");
        }

        // =====================================================
        // FORGOT PASSWORD
        // =====================================================
        public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Fail("User not found.");

            await InvalidateOldOtp(dto.Email);

            var otp = GenerateOtp();

            await _context.OtpVerifications.AddAsync(new OtpVerification
            {
                Email = dto.Email,
                OtpCode = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            });

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                dto.Email,
                "Password Reset OTP",
                $"<h3>Your OTP</h3><p><b>{otp}</b></p>"
            );

            return Success("Password reset OTP sent.");
        }

        // =====================================================
        // RESET PASSWORD
        // =====================================================
        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Fail("User not found.");

            var otp = await _context.OtpVerifications.FirstOrDefaultAsync(x =>
                x.Email == dto.Email &&
                x.OtpCode == dto.OtpCode &&
                !x.IsUsed &&
                x.ExpiryTime > DateTime.UtcNow);

            if (otp == null)
                return Fail("Invalid or expired OTP.");

            otp.IsUsed = true;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

            if (!result.Succeeded)
                return Fail(string.Join(" | ", result.Errors.Select(e => e.Description)));

            await _context.SaveChangesAsync();
            return Success("Password reset successfully.");
        }

        // =====================================================
        // PRIVATE HELPERS
        // =====================================================
        private async Task<(string Token, DateTime Expiry)> GenerateJwtToken(ApplicationUser user)
        {
            var jwt = _config.GetSection("Jwt");

            var key = Environment.GetEnvironmentVariable("JWT_KEY")
                      ?? jwt["Key"]
                      ?? throw new Exception("JWT key missing");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.Name, user.UserName ?? "")
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["DurationInMinutes"]!)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }

        private async Task<RefreshToken> CreateRefreshToken(string userId)
        {
            var token = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            return token;
        }

        private async Task InvalidateOldOtp(string email)
        {
            var oldOtp = await _context.OtpVerifications
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsUsed);

            if (oldOtp != null)
                _context.OtpVerifications.Remove(oldOtp);
        }

        private static string GenerateOtp()
            => new Random().Next(100000, 999999).ToString();

        private static AuthResponseDto Fail(string msg)
            => new() { Success = false, Message = msg };

        private static AuthResponseDto Success(string msg)
            => new() { Success = true, Message = msg };

        private static LoginResponseDto LoginFail(string msg)
            => new() { Success = false, Message = msg };
        private SymmetricSecurityKey GetJwtKey()
        {
            var jwtKey =
                Environment.GetEnvironmentVariable("JWT_KEY")
                ?? _config["Jwt:Key"]
                ?? throw new Exception("JWT key not configured");

            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        }

    }
}
