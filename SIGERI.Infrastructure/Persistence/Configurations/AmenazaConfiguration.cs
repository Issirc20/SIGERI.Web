using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class AmenazaConfiguration : IEntityTypeConfiguration<Amenaza>
{
    public void Configure(EntityTypeBuilder<Amenaza> entity)
    {
        entity.ToTable("Amenazas");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Codigo).HasMaxLength(50).IsRequired();
        entity.HasIndex(e => e.Codigo).IsUnique();
        entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Descripcion).HasMaxLength(1000).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasOne(e => e.Categoria)
            .WithMany(e => e.Amenazas)
            .HasForeignKey(e => e.CategoriaAmenazaId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(e => e.Riesgos)
            .WithOne(e => e.Amenaza)
            .HasForeignKey(e => e.AmenazaId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
