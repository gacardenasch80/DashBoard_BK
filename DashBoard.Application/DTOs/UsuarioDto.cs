namespace DashBoard.Application.DTOs;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public bool Activo { get; set; }
}

public class CreateUsuarioDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateUsuarioDto
{
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Password { get; set; }
}
