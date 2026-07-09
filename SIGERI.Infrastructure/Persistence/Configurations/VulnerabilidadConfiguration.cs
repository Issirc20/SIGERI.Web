using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class VulnerabilidadConfiguration : IEntityTypeConfiguration<Vulnerabilidad>
{
    public void Configure(EntityTypeBuilder<Vulnerabilidad> entity)
    {
        entity.ToTable("Vulnerabilidades");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
        entity.HasIndex(e => e.Codigo).IsUnique();
        entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.SeveridadBase).HasMaxLength(50).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasMany(e => e.Riesgos)
            .WithOne(e => e.Vulnerabilidad)
            .HasForeignKey(e => e.VulnerabilidadId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
