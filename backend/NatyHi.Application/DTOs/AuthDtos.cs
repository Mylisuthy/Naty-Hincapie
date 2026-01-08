namespace NatyHi.Application.DTOs;

public record RegisterUserDto(string Email, string Password);
public record LoginUserDto(string Email, string Password);
public record TokenResponseDto(string AccessToken, string RefreshToken);
