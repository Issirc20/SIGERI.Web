using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class EvaluacionConfiguration : IEntityTypeConfiguration<Evaluacion>
{
    public void Configure(EntityTypeBuilder<Evaluacion> entity)
    {
        entity.ToTable("Evaluaciones");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Fecha).HasColumnType("datetime2").IsRequired();
        entity.Property(e => e.Observaciones).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property<bool>(nameof(BaseEntity.Activo)).HasDefaultValue(true);

        entity.HasOne(e => e.Usuario)
            .WithMany(e => e.Evaluaciones)
            .HasForeignKey(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.ActivoRelacionado)
            .WithMany(e => e.Evaluaciones)
            .HasForeignKey(e => e.ActivoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Riesgos)
            .WithOne(e => e.Evaluacion)
            .HasForeignKey(e => e.EvaluacionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => EF.Property<bool>(e, nameof(BaseEntity.Activo)));
    }
}
