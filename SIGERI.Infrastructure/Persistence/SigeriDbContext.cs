using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Interfaces;
using SIGERI.Domain.Entities;

namespace SIGERI.Infrastructure.Persistence;

public sealed class SigeriDbContext : DbContext, ISigeriDbContext
{
    public SigeriDbContext(DbContextOptions<SigeriDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Activo> Activos => Set<Activo>();
    public DbSet<DependenciaActivo> DependenciasActivos => Set<DependenciaActivo>();
    public DbSet<PerfilActivoCritico> PerfilesActivosCriticos => Set<PerfilActivoCritico>();
    public DbSet<ControlMadurez> ControlesMadurez => Set<ControlMadurez>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<Riesgo> Riesgos => Set<Riesgo>();
    public DbSet<Amenaza> Amenazas => Set<Amenaza>();
    public DbSet<Vulnerabilidad> Vulnerabilidades => Set<Vulnerabilidad>();
    public DbSet<PlanTratamiento> PlanesTratamiento => Set<PlanTratamiento>();
    public DbSet<Control> Controles => Set<Control>();
    public DbSet<HistorialRiesgoResidual> HistorialesRiesgoResidual => Set<HistorialRiesgoResidual>();
    public DbSet<Organizacion> Organizaciones => Set<Organizacion>();
    public DbSet<CategoriaAmenaza> CategoriasAmenaza => Set<CategoriaAmenaza>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
