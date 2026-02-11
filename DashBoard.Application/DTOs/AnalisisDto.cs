namespace DashBoard.Application.DTOs;

public class AnalisisDto
{
    public Guid Id { get; set; }
    public string NombreAnalisis { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public int TotalFacturas { get; set; }
    public decimal ValorTotal { get; set; }
}

public class AnalisisResponseDto
{
    public Guid Id { get; set; }
    public string NombreAnalisis { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public string JsonData { get; set; } = string.Empty;
    public string? FiltrosAplicados { get; set; }
    public int TotalFacturas { get; set; }
    public decimal ValorTotal { get; set; }
}

public class CreateAnalisisDto
{
    public string NombreAnalisis { get; set; } = string.Empty;
    public object JsonData { get; set; } = null!;
    public object? FiltrosAplicados { get; set; }
    public int TotalFacturas { get; set; }
    public decimal ValorTotal { get; set; }
}
