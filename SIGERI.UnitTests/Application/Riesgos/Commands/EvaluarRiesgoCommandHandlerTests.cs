using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SIGERI.Application.Interfaces;
using SIGERI.Application.Riesgos.Commands;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Infrastructure.Persistence;
using Xunit;

namespace SIGERI.UnitTests.Application.Riesgos.Commands;

public sealed class EvaluarRiesgoCommandHandlerTests
{
    [Fact]
    public async Task Handle_con_datos_validos_debe_crear_el_riesgo_y_retorna_su_identificador()
    {
        var setup = CreateSetup();
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        var result = await setup.Handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        setup.RiesgosAgregados.Should().ContainSingle();

        var riesgoCreado = setup.RiesgosAgregados.Single();
        result.Should().Be(riesgoCreado.Id);
        riesgoCreado.Nombre.Should().Be(command.NombreRiesgo.Trim());
        riesgoCreado.Descripcion.Should().Be(command.DescripcionRiesgo.Trim());
        riesgoCreado.PuntajeRiesgo.Should().Be(16);
        riesgoCreado.NivelRiesgo.Should().Be("Crítico");
        riesgoCreado.Estado.Should().Be(EstadoRiesgo.Evaluado);
        riesgoCreado.EvaluacionId.Should().Be(command.EvaluacionId);
        riesgoCreado.AmenazaId.Should().Be(command.AmenazaId);
        riesgoCreado.VulnerabilidadId.Should().Be(command.VulnerabilidadId);
        riesgoCreado.CreadoPor.Should().Be(command.CreadoPor.Trim());
    }

    [Fact]
    public async Task Handle_con_probabilidad_invalida_debe_lanzar_DomainException()
    {
        var setup = CreateSetup();
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId, probabilidad: 0);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La probabilidad debe estar entre 1 y 5.");
    }

    [Fact]
    public async Task Handle_con_impacto_invalido_debe_lanzar_DomainException()
    {
        var setup = CreateSetup();
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId, impacto: 6);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El impacto debe estar entre 1 y 5.");
    }

    [Fact]
    public async Task Handle_con_activo_inexistente_debe_lanzar_DomainException()
    {
        var setup = CreateSetup(includeActivo: false, includeEvaluacion: false, includeAmenaza: false, includeVulnerabilidad: false);
        var command = CreateCommand(Guid.NewGuid(), setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo especificado no existe.");
    }

    [Fact]
    public async Task Handle_con_evaluacion_inexistente_debe_lanzar_DomainException()
    {
        var setup = CreateSetup(includeEvaluacion: false);
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La evaluación especificada no existe.");
    }

    [Fact]
    public async Task Handle_con_evaluacion_de_otro_activo_debe_lanzar_DomainException()
    {
        var setup = CreateSetup(evaluacionPerteneceAlActivo: false);
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La evaluación no corresponde al activo especificado.");
    }

    [Fact]
    public async Task Handle_con_amenaza_inexistente_debe_lanzar_DomainException()
    {
        var setup = CreateSetup(includeAmenaza: false);
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La amenaza especificada no existe.");
    }

    [Fact]
    public async Task Handle_con_vulnerabilidad_inexistente_debe_lanzar_DomainException()
    {
        var setup = CreateSetup(includeVulnerabilidad: false);
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        await FluentActions.Invoking(() => setup.Handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La vulnerabilidad especificada no existe.");
    }

    [Fact]
    public async Task Handle_con_datos_validos_debe_invocar_SaveChangesAsync_una_sola_vez()
    {
        var setup = CreateSetup();
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId);

        await setup.Handler.Handle(command, CancellationToken.None);

        setup.ContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static TestSetup CreateSetup(bool includeActivo = true, bool includeEvaluacion = true, bool includeAmenaza = true, bool includeVulnerabilidad = true, bool evaluacionPerteneceAlActivo = true)
    {
        var options = new DbContextOptionsBuilder<SigeriDbContext>()
            .UseInMemoryDatabase($"SIGERI_UnitTests_{Guid.NewGuid():N}")
            .Options;

        var realContext = new SigeriDbContext(options);

        var organizacion = new Organizacion("SIGERI Corp", "SIGERI", new List<Activo>());
        var activoId = Guid.NewGuid();
        var activo = new Activo("ACT-001", "SCADA Planta Norte", "Sistema de control industrial", TipoActivo.Software, "Jefe OT", "Planta Norte", Criticidad.Critica, true, organizacion.Id, organizacion, new List<Evaluacion>())
        {
            Id = activoId
        };

        var usuario = new Usuario("Ana", "Pérez", "ana@sigeri.local", "hash", RolUsuario.Administrador, true, new List<Evaluacion>());
        var evaluacionId = Guid.NewGuid();
        var activoEvaluacion = evaluacionPerteneceAlActivo ? activo : new Activo("ACT-999", "Activo auxiliar", "Activo para relación inválida", TipoActivo.Software, "Jefe OT", "Planta Norte", Criticidad.Alta, true, organizacion.Id, organizacion, new List<Evaluacion>())
        {
            Id = Guid.NewGuid()
        };
        var evaluacion = new Evaluacion(DateTime.UtcNow, "Evaluación inicial", usuario.Id, usuario, activoEvaluacion.Id, activoEvaluacion, new List<Riesgo>())
        {
            Id = evaluacionId
        };

        var categoria = new CategoriaAmenaza("Ciberamenazas", "Amenazas asociadas a operación digital", new List<Amenaza>());
        var amenazaId = Guid.NewGuid();
        var amenaza = new Amenaza("AM-001", "Ransomware", "Ataque de cifrado", categoria.Id, categoria, new List<Riesgo>())
        {
            Id = amenazaId
        };

        var vulnerabilidadId = Guid.NewGuid();
        var vulnerabilidad = new Vulnerabilidad("VUL-001", "Servicios expuestos", "Exposición de servicios críticos", "Alta", new List<Riesgo>())
        {
            Id = vulnerabilidadId
        };

        if (includeActivo)
        {
            realContext.Organizaciones.Add(organizacion);
            realContext.Activos.Add(activo);
        }

        if (includeEvaluacion)
        {
            if (!includeActivo)
            {
                realContext.Organizaciones.Add(organizacion);
            }

            if (!evaluacionPerteneceAlActivo)
            {
                realContext.Activos.Add(activoEvaluacion);
            }

            realContext.Evaluaciones.Add(evaluacion);
        }

        if (includeAmenaza)
        {
            realContext.CategoriasAmenaza.Add(categoria);
            realContext.Amenazas.Add(amenaza);
        }

        if (includeVulnerabilidad)
        {
            realContext.Vulnerabilidades.Add(vulnerabilidad);
        }

        realContext.SaveChanges();

        var riesgosAgregados = new List<Riesgo>();
        var riesgosMock = new Mock<DbSet<Riesgo>>();
        riesgosMock
            .Setup(x => x.Add(It.IsAny<Riesgo>()))
            .Returns<Riesgo>(riesgo =>
            {
                riesgosAgregados.Add(riesgo);
                return null!;
            });

        var contextMock = new Mock<ISigeriDbContext>();
        contextMock.SetupGet(x => x.Activos).Returns(realContext.Activos);
        contextMock.SetupGet(x => x.Evaluaciones).Returns(realContext.Evaluaciones);
        contextMock.SetupGet(x => x.Amenazas).Returns(realContext.Amenazas);
        contextMock.SetupGet(x => x.Vulnerabilidades).Returns(realContext.Vulnerabilidades);
        contextMock.SetupGet(x => x.Riesgos).Returns(riesgosMock.Object);
        contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return new TestSetup(
            new EvaluarRiesgoCommandHandler(contextMock.Object),
            contextMock,
            riesgosAgregados,
            activoId,
            evaluacionId,
            amenazaId,
            vulnerabilidadId);
    }

    private static EvaluarRiesgoCommand CreateCommand(Guid activoId, Guid evaluacionId, Guid amenazaId, Guid vulnerabilidadId, int probabilidad = 4, int impacto = 4)
        => new(
            ActivoId: activoId,
            EvaluacionId: evaluacionId,
            AmenazaId: amenazaId,
            VulnerabilidadId: vulnerabilidadId,
            Probabilidad: probabilidad,
            Impacto: impacto,
            NombreRiesgo: "  Riesgo de Ransomware en SCADA  ",
            DescripcionRiesgo: "  Impacto sobre operación industrial  ",
            CreadoPor: "  qa.bot  ");

    private sealed record TestSetup(
        EvaluarRiesgoCommandHandler Handler,
        Mock<ISigeriDbContext> ContextMock,
        List<Riesgo> RiesgosAgregados,
        Guid ActivoId,
        Guid EvaluacionId,
        Guid AmenazaId,
        Guid VulnerabilidadId);
}
