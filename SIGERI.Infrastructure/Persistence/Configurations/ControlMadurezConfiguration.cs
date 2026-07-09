using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class ControlMadurezConfiguration : IEntityTypeConfiguration<ControlMadurez>
{
    public void Configure(EntityTypeBuilder<ControlMadurez> entity)
    {
        entity.ToTable("ControlesMadurez");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
        entity.HasIndex(e => e.Codigo).IsUnique();
        entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Funcion).IsRequired();
        entity.Property(e => e.NivelActual).IsRequired();
        entity.Property(e => e.NivelObjetivo).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasQueryFilter(e => e.Activo);
    }
}
