using BlogAPI.Domain.DTOs.Auth;

namespace BlogAPI.Domain.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task<ApiResponse<string>> RevokeTokenAsync(string userId);
}