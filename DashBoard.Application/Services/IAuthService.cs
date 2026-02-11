using DashBoard.Application.DTOs;

namespace DashBoard.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<UsuarioDto> GetCurrentUserAsync(string username);
}
