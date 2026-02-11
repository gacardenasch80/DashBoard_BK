using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashBoard.Core.Entities;

public class Analisis
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, StringLength(200)]
    public string NombreAnalisis { get; set; } = string.Empty;

    [Required]
    public Guid UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }

    public string JsonData { get; set; } = string.Empty;
    public string? FiltrosAplicados { get; set; }

    public int TotalFacturas { get; set; }
    public decimal ValorTotal { get; set; }
}
