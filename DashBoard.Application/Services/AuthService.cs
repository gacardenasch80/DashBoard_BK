using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using DashBoard.Application.DTOs;
using DashBoard.Core.Interfaces;

namespace DashBoard.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuarios = await _unitOfWork.Usuarios.FindAsync(u => u.Username == request.Username);
        var usuario = usuarios.FirstOrDefault();

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        if (!usuario.Activo)
            throw new UnauthorizedAccessException("Usuario inactivo");

        var token = GenerateJwtToken(usuario);

        return new LoginResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(8),
            Usuario = _mapper.Map<UsuarioDto>(usuario)
        };
    }

    public async Task<UsuarioDto> GetCurrentUserAsync(string username)
    {
        var usuarios = await _unitOfWork.Usuarios.FindAsync(u => u.Username == username);
        var usuario = usuarios.FirstOrDefault();

        if (usuario == null)
            throw new KeyNotFoundException("Usuario no encontrado");

        return _mapper.Map<UsuarioDto>(usuario);
    }

    private string GenerateJwtToken(Core.Entities.Usuario usuario)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.GivenName, usuario.Nombres),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
