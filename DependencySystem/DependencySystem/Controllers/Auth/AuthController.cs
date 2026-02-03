using DependencySystem.DTOs.Auth;
using DependencySystem.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace DependencySystem.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(SendOtpRequestDto dto)
        {
            var result = await _authService.SendOtpAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
