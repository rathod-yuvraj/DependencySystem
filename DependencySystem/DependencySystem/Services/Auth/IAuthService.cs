using DependencySystem.DTOs.Auth;

namespace DependencySystem.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    }
}
