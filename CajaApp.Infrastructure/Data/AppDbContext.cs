using CajaApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CajaApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Caja> Cajas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<MovimientoCaja> Movimientos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Caja
        modelBuilder.Entity<Caja>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Descripcion).HasMaxLength(500);
            entity.Property(c => c.SaldoActual).HasColumnType("decimal(18,2)");
            entity.Property(c => c.Estado).HasConversion<int>();
            entity.Property(c => c.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(c => c.Version).IsRowVersion();
        });

        // Configuración de Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Apellidos).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).HasMaxLength(100).UseCollation("NOCASE");
            entity.Property(c => c.Telefono).HasMaxLength(20);
            entity.Property(c => c.Direccion).HasMaxLength(500);
            entity.Property(c => c.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(c => c.Email).IsUnique();
        });

        // Configuración de MovimientoCaja
        modelBuilder.Entity<MovimientoCaja>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Tipo).HasConversion<int>();
            entity.Property(m => m.Monto).HasColumnType("decimal(18,2)");
            entity.Property(m => m.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(m => m.Categoria).HasMaxLength(100);
            entity.Property(m => m.Referencia).HasMaxLength(100);
            entity.Property(m => m.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(m => m.Caja)
                .WithMany()
                .HasForeignKey(m => m.CajaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Cliente)
                .WithMany()
                .HasForeignKey(m => m.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(m => new { m.CajaId, m.Fecha });
            entity.HasIndex(m => m.Tipo);
            entity.HasIndex(m => m.ClienteId);
        });

        // Datos iniciales
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Caja por defecto
        modelBuilder.Entity<Caja>().HasData(new Caja
        {
            Id = 1,
            Nombre = "Caja Principal",
            Descripcion = "Caja principal del negocio",
            SaldoActual = 0m,
            Estado = Domain.Enums.EstadoCaja.Cerrada,
            FechaCreacion = DateTime.UtcNow
        });
    }
}
