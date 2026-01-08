using NatyHi.Application.DTOs;

namespace NatyHi.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<TokenResponseDto> LoginAsync(LoginUserDto dto);
    Task<TokenResponseDto> RefreshTokenAsync(string token, string refreshToken);
}
