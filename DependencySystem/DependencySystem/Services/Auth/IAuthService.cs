using DependencySystem.DTOs.Auth;

namespace DependencySystem.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> SendOtpAsync(SendOtpRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);

        Task<AuthResponseDto> VerifyOtpAsync(VerifyOtpRequestDto dto);


        // 🔐 Reset Password
        Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
        Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordRequestDto dto);
    }
}
