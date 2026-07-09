using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class DependenciaActivoConfiguration : IEntityTypeConfiguration<DependenciaActivo>
{
    public void Configure(EntityTypeBuilder<DependenciaActivo> entity)
    {
        entity.ToTable("DependenciasActivos");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.TipoDependencia).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.CriticidadOperativa).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasOne(e => e.ActivoOrigen)
            .WithMany(e => e.DependenciasOrigen)
            .HasForeignKey(e => e.ActivoOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.ActivoDestino)
            .WithMany(e => e.DependenciasDestino)
            .HasForeignKey(e => e.ActivoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
