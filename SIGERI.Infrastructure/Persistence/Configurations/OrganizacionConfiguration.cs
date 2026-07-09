using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class OrganizacionConfiguration : IEntityTypeConfiguration<Organizacion>
{
    public void Configure(EntityTypeBuilder<Organizacion> entity)
    {
        entity.ToTable("Organizaciones");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.Nombre).HasMaxLength(200).IsRequired();
        entity.Property(e => e.Siglas).HasMaxLength(50).IsRequired();
        entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
        entity.Property(e => e.ActualizadoPor).HasMaxLength(100);
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasMany(e => e.Activos)
            .WithOne(e => e.Organizacion)
            .HasForeignKey(e => e.OrganizacionId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
