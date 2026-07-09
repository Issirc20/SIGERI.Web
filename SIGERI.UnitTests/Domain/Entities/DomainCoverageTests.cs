using FluentAssertions;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Domain.ValueObjects;
using Xunit;

namespace SIGERI.UnitTests.Domain.Entities;

public sealed class DomainCoverageTests
{
    [Fact]
    public void DomainException_debe_construirse_con_todas_las_sobrecargas()
    {
        var ex1 = new DomainException();
        var ex2 = new DomainException("mensaje");
        var ex3 = new DomainException("mensaje", new InvalidOperationException("inner"));

        ex1.Should().NotBeNull();
        ex2.Message.Should().Be("mensaje");
        ex3.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void ValueObjects_deben_asignar_propiedades_en_constructores()
    {
        var impactoDefault = new Impacto();
        var impacto = new Impacto(4, "Alto");
        var probDefault = new Probabilidad();
        var probabilidad = new Probabilidad(3, "Media");

        impactoDefault.Descripcion.Should().BeEmpty();
        impacto.Valor.Should().Be(4);
        probDefault.Descripcion.Should().BeEmpty();
        probabilidad.Descripcion.Should().Be("Media");
    }

    [Fact]
    public void Entidades_deben_inicializar_constructores_default_y_parametrizados()
    {
        var organizacion = new Organizacion("SIGERI", "SIG", new List<Activo>());
        var usuario = new Usuario("Ana", "Pérez", "ana@sigeri.local", "hash", RolUsuario.AnalistaRiesgos, true, new List<Evaluacion>());
        var activo = new Activo("A1", "Activo", "Desc", TipoActivo.Hardware, "Owner", "Lima", Criticidad.Alta, true, organizacion.Id, organizacion, new List<Evaluacion>());
        var categoria = new CategoriaAmenaza("Cat", "Desc", new List<Amenaza>());
        var amenaza = new Amenaza("AM1", "Amenaza", "Desc", categoria.Id, categoria, new List<Riesgo>());
        var vulnerabilidad = new Vulnerabilidad("V1", "Vuln", "Desc", "Alta", new List<Riesgo>());
        var evaluacion = new Evaluacion(DateTime.UtcNow, "Obs", usuario.Id, usuario, activo.Id, activo, new List<Riesgo>());
        var riesgo = new Riesgo("R1", "Desc", new Probabilidad(2, "Baja"), new Impacto(3, "Medio"), 6, "Medio", EstadoRiesgo.Evaluado, evaluacion.Id, evaluacion, amenaza.Id, amenaza, vulnerabilidad.Id, vulnerabilidad, new List<PlanTratamiento>(), new List<HistorialRiesgoResidual>());
        var plan = new PlanTratamiento(riesgo.Id, riesgo, "Mitigar", "Aplicar controles", DateTime.UtcNow, DateTime.UtcNow.AddDays(5), EstadoPlan.Pendiente, new List<Control>());
        var control = new Control(plan.Id, plan, "C1", "Control", "Desc", true, false);
        var historial = new HistorialRiesgoResidual(riesgo.Id, riesgo, 10, 5, DateTime.UtcNow);
        var dependencia = new DependenciaActivo(activo.Id, activo, activo.Id, activo, TipoDependenciaActivo.Tecnologica, "Desc", 3);
        var perfil = new PerfilActivoCritico(activo.Id, activo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, NivelImpactoOctave.Alto, NivelImpactoOctave.Medio, NivelImpactoOctave.Bajo, "Tec", "Fis", "Hum");
        var madurez = new ControlMadurez("CIS-1", "Inventario", FuncionNist.Identify, 2, 4, "Desc");

        new Organizacion().Nombre.Should().BeEmpty();
        new CategoriaAmenaza().Amenazas.Should().BeEmpty();
        new Amenaza().Riesgos.Should().BeEmpty();
        new Usuario().Evaluaciones.Should().BeEmpty();
        new Activo().PerfilesCriticos.Should().BeEmpty();
        new Evaluacion().Riesgos.Should().BeEmpty();
        new Riesgo().Historiales.Should().BeEmpty();
        new PlanTratamiento().Controles.Should().BeEmpty();
        new Control().Codigo.Should().BeEmpty();
        new HistorialRiesgoResidual().Should().NotBeNull();
        new DependenciaActivo().Descripcion.Should().BeEmpty();
        new PerfilActivoCritico().ContenedoresHumanos.Should().BeEmpty();
        new ControlMadurez().Nombre.Should().BeEmpty();
        new Vulnerabilidad().SeveridadBase.Should().BeEmpty();

        organizacion.Siglas.Should().Be("SIG");
        usuario.Rol.Should().Be(RolUsuario.AnalistaRiesgos);
        activo.Codigo.Should().Be("A1");
        categoria.Nombre.Should().Be("Cat");
        amenaza.Codigo.Should().Be("AM1");
        vulnerabilidad.Codigo.Should().Be("V1");
        evaluacion.Observaciones.Should().Be("Obs");
        riesgo.NivelRiesgo.Should().Be("Medio");
        plan.Estrategia.Should().Be("Mitigar");
        control.MitigaProbabilidad.Should().BeTrue();
        historial.PuntajeResidual.Should().Be(5);
        dependencia.CriticidadOperativa.Should().Be(3);
        perfil.ContenedoresTecnicos.Should().Be("Tec");
        madurez.Funcion.Should().Be(FuncionNist.Identify);
    }
}
