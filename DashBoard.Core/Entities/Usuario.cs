using System.ComponentModel.DataAnnotations;

namespace DashBoard.Core.Entities;

public class Usuario
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string Password { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<Analisis> Analisis { get; set; } = new List<Analisis>();
}
