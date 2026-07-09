using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class PlanTratamientoConfiguration : IEntityTypeConfiguration<PlanTratamiento>
{
    public void Configure(EntityTypeBuilder<PlanTratamiento> entity)
    {
        entity.ToTable("PlanesTratamiento");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Estrategia).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.FechaInicio).HasColumnType("datetime2").IsRequired();
        entity.Property(e => e.FechaTermino).HasColumnType("datetime2").IsRequired();
        entity.Property(e => e.Estado).IsRequired();
        entity.Property(e => e.CostoSalvaguarda).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        entity.Property(e => e.PorcentajeMitigacion).HasColumnType("decimal(5,2)").HasDefaultValue(0m);
        entity.Property(e => e.AleBase).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasOne(e => e.Riesgo)
            .WithMany(e => e.PlanesTratamiento)
            .HasForeignKey(e => e.RiesgoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Controles)
            .WithOne(e => e.PlanTratamiento)
            .HasForeignKey(e => e.PlanTratamientoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
