using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> entity)
    {
        entity.ToTable("Usuarios");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        entity.Property(e => e.Apellido).HasMaxLength(150).IsRequired();
        entity.Property(e => e.Correo).HasMaxLength(256).IsRequired();
        entity.HasIndex(e => e.Correo).IsUnique();
        entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
        entity.Property(e => e.Rol).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasMany(e => e.Evaluaciones)
            .WithOne(e => e.Usuario)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
