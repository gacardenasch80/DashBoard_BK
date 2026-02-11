using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DashBoard.Application.DTOs;
using DashBoard.Application.Services;
using System.Security.Claims;

namespace DashBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Iniciar sesión y obtener token JWT
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtener información del usuario actual
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Token inválido" });

            var usuario = await _authService.GetCurrentUserAsync(username);
            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", details = ex.Message });
        }
    }

    /// <summary>
    /// Verificar si el token es válido
    /// </summary>
    [HttpGet("verify")]
    [Authorize]
    public IActionResult VerifyToken()
    {
        return Ok(new { valid = true, message = "Token válido" });
    }
}
