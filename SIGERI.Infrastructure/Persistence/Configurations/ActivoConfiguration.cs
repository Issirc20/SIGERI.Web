using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class ActivoConfiguration : IEntityTypeConfiguration<Activo>
{
    public void Configure(EntityTypeBuilder<Activo> entity)
    {
        entity.ToTable("Activos");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
        entity.HasIndex(e => e.Codigo).IsUnique();
        entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.Tipo).IsRequired();
        entity.Property(e => e.Propietario).HasMaxLength(150).IsRequired();
        entity.Property(e => e.Ubicacion).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Criticidad).IsRequired();
        entity.Property(e => e.Estado).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasOne(e => e.Organizacion)
            .WithMany(e => e.Activos)
            .HasForeignKey(e => e.OrganizacionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Evaluaciones)
            .WithOne(e => e.ActivoRelacionado)
            .HasForeignKey(e => e.ActivoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.DependenciasOrigen)
            .WithOne(e => e.ActivoOrigen)
            .HasForeignKey(e => e.ActivoOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.DependenciasDestino)
            .WithOne(e => e.ActivoDestino)
            .HasForeignKey(e => e.ActivoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
