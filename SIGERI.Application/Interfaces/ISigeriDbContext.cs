using Microsoft.EntityFrameworkCore;
using SIGERI.Domain.Entities;

namespace SIGERI.Application.Interfaces;

public interface ISigeriDbContext
{
    DbSet<Usuario> Usuarios { get; }

    DbSet<Activo> Activos { get; }

    DbSet<DependenciaActivo> DependenciasActivos { get; }

    DbSet<PerfilActivoCritico> PerfilesActivosCriticos { get; }

    DbSet<ControlMadurez> ControlesMadurez { get; }

    DbSet<Evaluacion> Evaluaciones { get; }

    DbSet<Riesgo> Riesgos { get; }

    DbSet<Amenaza> Amenazas { get; }

    DbSet<Vulnerabilidad> Vulnerabilidades { get; }

    DbSet<PlanTratamiento> PlanesTratamiento { get; }

    DbSet<Control> Controles { get; }

    DbSet<HistorialRiesgoResidual> HistorialesRiesgoResidual { get; }

    DbSet<Organizacion> Organizaciones { get; }

    DbSet<CategoriaAmenaza> CategoriasAmenaza { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
