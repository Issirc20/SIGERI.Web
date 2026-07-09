using Microsoft.EntityFrameworkCore;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Infrastructure.Persistence;

namespace SIGERI.UnitTests.TestSupport;

internal sealed class TestDataContext
{
    public required SigeriDbContext Context { get; init; }
    public required Organizacion Organizacion { get; init; }
    public required Activo ActivoPrincipal { get; init; }
    public required Activo ActivoSecundario { get; init; }
    public required Usuario Usuario { get; init; }
    public required Evaluacion Evaluacion { get; init; }
    public required CategoriaAmenaza CategoriaAmenaza { get; init; }
    public required Amenaza Amenaza { get; init; }
    public required Vulnerabilidad Vulnerabilidad { get; init; }
    public required DependenciaActivo Dependencia { get; init; }
    public required PerfilActivoCritico Perfil { get; init; }
    public required ControlMadurez ControlMadurez { get; init; }
}

internal static class TestDbFactory
{
    public static TestDataContext Create()
    {
        var options = new DbContextOptionsBuilder<SigeriDbContext>()
            .UseInMemoryDatabase($"SIGERI_UnitTests_{Guid.NewGuid():N}")
            .Options;

        var context = new SigeriDbContext(options);

        var organizacion = new Organizacion("SIGERI Corp", "SIGERI", new List<Activo>());

        var activoPrincipal = new Activo(
            "ACT-001",
            "SCADA",
            "Sistema principal",
            TipoActivo.Software,
            "Jefe OT",
            "Planta Norte",
            Criticidad.Critica,
            true,
            organizacion.Id,
            organizacion,
            new List<Evaluacion>());

        var activoSecundario = new Activo(
            "ACT-002",
            "Servidor IAM",
            "Servidor de identidad",
            TipoActivo.Software,
            "CISO",
            "CPD",
            Criticidad.Alta,
            true,
            organizacion.Id,
            organizacion,
            new List<Evaluacion>());

        organizacion.Activos.Add(activoPrincipal);
        organizacion.Activos.Add(activoSecundario);

        var usuario = new Usuario("Ana", "Pérez", "ana@sigeri.local", "hash", RolUsuario.Administrador, true, new List<Evaluacion>());

        var evaluacion = new Evaluacion(DateTime.UtcNow, "Evaluación inicial", usuario.Id, usuario, activoPrincipal.Id, activoPrincipal, new List<Riesgo>());
        usuario.Evaluaciones.Add(evaluacion);
        activoPrincipal.Evaluaciones.Add(evaluacion);

        var categoriaAmenaza = new CategoriaAmenaza("Ciberamenazas", "Amenazas digitales", new List<Amenaza>());
        var amenaza = new Amenaza("AM-001", "Malware", "Código malicioso", categoriaAmenaza.Id, categoriaAmenaza, new List<Riesgo>());
        categoriaAmenaza.Amenazas.Add(amenaza);

        var vulnerabilidad = new Vulnerabilidad("VUL-001", "Credenciales débiles", "Contraseñas débiles", "Alta", new List<Riesgo>());

        var dependencia = new DependenciaActivo(
            activoPrincipal.Id,
            activoPrincipal,
            activoSecundario.Id,
            activoSecundario,
            TipoDependenciaActivo.Tecnologica,
            "Dependencia funcional",
            4)
        {
            CreadoPor = "seed"
        };

        var perfil = new PerfilActivoCritico(
            activoPrincipal.Id,
            activoPrincipal,
            NivelImpactoOctave.Alto,
            NivelImpactoOctave.Medio,
            NivelImpactoOctave.Alto,
            NivelImpactoOctave.Medio,
            NivelImpactoOctave.Alto,
            "SIEM",
            "CPD",
            "SOC")
        {
            CreadoPor = "seed"
        };

        var controlMadurez = new ControlMadurez("CIS-01", "Inventario", FuncionNist.Identify, 2, 4, "Inventario de activos")
        {
            CreadoPor = "seed"
        };

        context.Organizaciones.Add(organizacion);
        context.Activos.AddRange(activoPrincipal, activoSecundario);
        context.Usuarios.Add(usuario);
        context.Evaluaciones.Add(evaluacion);
        context.CategoriasAmenaza.Add(categoriaAmenaza);
        context.Amenazas.Add(amenaza);
        context.Vulnerabilidades.Add(vulnerabilidad);
        context.DependenciasActivos.Add(dependencia);
        context.PerfilesActivosCriticos.Add(perfil);
        context.ControlesMadurez.Add(controlMadurez);

        context.SaveChanges();

        return new TestDataContext
        {
            Context = context,
            Organizacion = organizacion,
            ActivoPrincipal = activoPrincipal,
            ActivoSecundario = activoSecundario,
            Usuario = usuario,
            Evaluacion = evaluacion,
            CategoriaAmenaza = categoriaAmenaza,
            Amenaza = amenaza,
            Vulnerabilidad = vulnerabilidad,
            Dependencia = dependencia,
            Perfil = perfil,
            ControlMadurez = controlMadurez
        };
    }
}
