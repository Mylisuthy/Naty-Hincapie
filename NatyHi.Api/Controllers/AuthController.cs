using Microsoft.AspNetCore.Mvc;
using NatyHi.Application.DTOs;
using NatyHi.Application.Interfaces;

namespace NatyHi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(new { AccessToken = result.AccessToken });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(new { AccessToken = result.AccessToken });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { Message = "No refresh token found." });
        }

        try 
        {
            // Aquí en un caso real enviaríamos el AccessToken expirado tb o extraeríamos el usuario del contexto
            var result = await _authService.RefreshTokenAsync("", refreshToken); 
            SetRefreshTokenCookie(result.RefreshToken);
             return Ok(new { AccessToken = result.AccessToken });
        }
         catch (Exception ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
            SameSite = SameSiteMode.Strict, // Ajustar según si front y back están en mismo dominio
            Secure = true // Requiere HTTPS
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
