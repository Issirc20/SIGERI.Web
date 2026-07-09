using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class RiesgoConfiguration : IEntityTypeConfiguration<Riesgo>
{
    public void Configure(EntityTypeBuilder<Riesgo> entity)
    {
        entity.ToTable("Riesgos");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.PuntajeRiesgo).IsRequired();
        entity.Property(e => e.NivelRiesgo).HasMaxLength(50).IsRequired();
        entity.Property(e => e.Estado).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.OwnsOne(e => e.Probabilidad, owned =>
        {
            owned.Property(p => p.Valor).HasColumnName("Probabilidad_Valor").IsRequired();
            owned.Property(p => p.Descripcion).HasColumnName("Probabilidad_Descripcion").HasMaxLength(200).IsRequired();
        });

        entity.OwnsOne(e => e.Impacto, owned =>
        {
            owned.Property(p => p.Valor).HasColumnName("Impacto_Valor").IsRequired();
            owned.Property(p => p.Descripcion).HasColumnName("Impacto_Descripcion").HasMaxLength(200).IsRequired();
        });

        entity.HasOne(e => e.Evaluacion)
            .WithMany(e => e.Riesgos)
            .HasForeignKey(e => e.EvaluacionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Amenaza)
            .WithMany(e => e.Riesgos)
            .HasForeignKey(e => e.AmenazaId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Vulnerabilidad)
            .WithMany(e => e.Riesgos)
            .HasForeignKey(e => e.VulnerabilidadId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.PlanesTratamiento)
            .WithOne(e => e.Riesgo)
            .HasForeignKey(e => e.RiesgoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Historiales)
            .WithOne(e => e.Riesgo)
            .HasForeignKey(e => e.RiesgoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
