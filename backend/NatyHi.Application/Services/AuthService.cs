using NatyHi.Application.DTOs;
using NatyHi.Application.Interfaces;
using NatyHi.Domain.Entities;
using NatyHi.Domain.Interfaces;

namespace NatyHi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
        {
            throw new Exception("El usuario ya existe."); // En un caso real usar una Custom Exception
        }

        var passwordHash = _passwordHasher.Hash(dto.Password);
        var user = new User(dto.Email, passwordHash);

        await _userRepository.AddAsync(user);

        return await GenerateTokensForUser(user);
    }

    public async Task<TokenResponseDto> LoginAsync(LoginUserDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
             throw new Exception("Credenciales inválidas.");
        }

        return await GenerateTokensForUser(user);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        // Nota: Aquí deberíamos validar el AccessToken expirado (claims) si es necesario,
        // pero principalmente validamos el RefreshToken contra la BD.
        
        // Esta lógica simplificada asume que el token viene en el request
        // En un caso real decodificamos el token para sacar el UserId o Email.
        // Por simplicidad, asumiremos que el frontend o el contexto nos da el usuario,
        // O buscamos al usuario que tenga este Refresh Token (si la BD lo permite indexar).
        
        // Para este ejemplo didáctico, asumiremos que "token" es el ID del usuario o el email (bad practice) 
        // O mejor: El RefreshToken en BDD está asociado al usuario. 
        // Implementación estricta: Extaer ID del expired token.
        
        // TODO: Implementar validación estricta de token expirado.
        throw new NotImplementedException("Implementaremos esto con la validación de JWT real.");
    }

    private async Task<TokenResponseDto> GenerateTokensForUser(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _userRepository.UpdateAsync(user);

        return new TokenResponseDto(accessToken, refreshToken);
    }
}
