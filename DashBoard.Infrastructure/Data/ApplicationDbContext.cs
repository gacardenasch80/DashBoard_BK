using Microsoft.EntityFrameworkCore;
using DashBoard.Core.Entities;

namespace DashBoard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Analisis> Analisis { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);

            entity.HasMany(e => e.Analisis)
                .WithOne(e => e.Usuario)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración Analisis
        modelBuilder.Entity<Analisis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.NombreAnalisis);
            entity.HasIndex(e => e.FechaCreacion);
            entity.Property(e => e.NombreAnalisis).IsRequired().HasMaxLength(200);
            entity.Property(e => e.JsonData).IsRequired();
            entity.Property(e => e.FiltrosAplicados);
            entity.Property(e => e.ValorTotal).HasColumnType("decimal(18,2)");
        });

        // SEED DATA - Usuario Admin por defecto
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = adminId,
                Nombres = "Administrador",
                Apellidos = "Sistema",
                Username = "Admin",
                Password = hashedPassword,
                Activo = true,
                FechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
