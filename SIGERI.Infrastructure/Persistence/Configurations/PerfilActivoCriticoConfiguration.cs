using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class PerfilActivoCriticoConfiguration : IEntityTypeConfiguration<PerfilActivoCritico>
{
    public void Configure(EntityTypeBuilder<PerfilActivoCritico> entity)
    {
        entity.ToTable("PerfilesActivosCriticos");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.ImpactoReputacion).IsRequired();
        entity.Property(e => e.ImpactoFinanciero).IsRequired();
        entity.Property(e => e.ImpactoProductividad).IsRequired();
        entity.Property(e => e.ImpactoLegal).IsRequired();
        entity.Property(e => e.ImpactoSeguridadSsoma).IsRequired();
        entity.Property(e => e.ContenedoresTecnicos).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.ContenedoresFisicos).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.ContenedoresHumanos).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasOne(e => e.ActivoRelacionado)
            .WithMany(e => e.PerfilesCriticos)
            .HasForeignKey(e => e.ActivoId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasQueryFilter(e => e.Activo);
    }
}
