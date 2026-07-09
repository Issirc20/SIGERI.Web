using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence.Configurations;

internal sealed class HistorialRiesgoResidualConfiguration : IEntityTypeConfiguration<HistorialRiesgoResidual>
{
    public void Configure(EntityTypeBuilder<HistorialRiesgoResidual> entity)
    {
        entity.ToTable("HistorialesRiesgoResidual");
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.PuntajeOriginal).IsRequired();
        entity.Property(e => e.PuntajeResidual).IsRequired();
        entity.Property(e => e.FechaCalculo).HasColumnType("datetime2").IsRequired();
        entity.Property(e => e.FechaCreacion).HasColumnType("datetime2");
        entity.Property(e => e.FechaActualizacion).HasColumnType("datetime2");
        entity.Property(e => e.Activo).HasDefaultValue(true);

        entity.HasOne(e => e.Riesgo)
            .WithMany(e => e.Historiales)
            .HasForeignKey(e => e.RiesgoId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasQueryFilter(e => e.Activo);
    }
}
