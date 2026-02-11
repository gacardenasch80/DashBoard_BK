namespace DashBoard.Application.DTOs;

public class UpdateAnalisisDto
{
    public string? NombreAnalisis { get; set; }
    public object? FiltrosAplicados { get; set; }
    public int? TotalFacturas { get; set; }
    public decimal? ValorTotal { get; set; }
}
