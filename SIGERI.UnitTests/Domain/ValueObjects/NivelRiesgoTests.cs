using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SIGERI.Application.Interfaces;
using SIGERI.Application.Riesgos.Commands;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Infrastructure.Persistence;
using Xunit;

namespace SIGERI.UnitTests.Domain.ValueObjects;

public sealed class NivelRiesgoTests
{
    [Theory]
    [InlineData(1, 1, "Bajo")]
    [InlineData(2, 2, "Bajo")]
    [InlineData(1, 5, "Medio")]
    [InlineData(3, 3, "Medio")]
    [InlineData(2, 5, "Alto")]
    [InlineData(3, 5, "Alto")]
    [InlineData(4, 4, "Crítico")]
    [InlineData(5, 5, "Crítico")]
    public async Task EvaluarRiesgo_debe_asignar_el_nivel_matriz_correcto(int probabilidad, int impacto, string nivelEsperado)
    {
        var setup = CreateSetup();
        var command = CreateCommand(setup.ActivoId, setup.EvaluacionId, setup.AmenazaId, setup.VulnerabilidadId, probabilidad, impacto);

        await setup.Handler.Handle(command, CancellationToken.None);

        setup.RiesgosAgregados.Should().ContainSingle();
        setup.RiesgosAgregados.Single().NivelRiesgo.Should().Be(nivelEsperado);
        setup.RiesgosAgregados.Single().PuntajeRiesgo.Should().Be(probabilidad * impacto);
    }

    private static TestSetup CreateSetup()
    {
        var options = new DbContextOptionsBuilder<SigeriDbContext>()
            .UseInMemoryDatabase($"SIGERI_UnitTests_Domain_{Guid.NewGuid():N}")
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
        var evaluacion = new Evaluacion(DateTime.UtcNow, "Evaluación inicial", usuario.Id, usuario, activo.Id, activo, new List<Riesgo>())
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

        realContext.Organizaciones.Add(organizacion);
        realContext.Activos.Add(activo);
        realContext.Evaluaciones.Add(evaluacion);
        realContext.CategoriasAmenaza.Add(categoria);
        realContext.Amenazas.Add(amenaza);
        realContext.Vulnerabilidades.Add(vulnerabilidad);
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

    private static EvaluarRiesgoCommand CreateCommand(Guid activoId, Guid evaluacionId, Guid amenazaId, Guid vulnerabilidadId, int probabilidad, int impacto)
        => new(
            ActivoId: activoId,
            EvaluacionId: evaluacionId,
            AmenazaId: amenazaId,
            VulnerabilidadId: vulnerabilidadId,
            Probabilidad: probabilidad,
            Impacto: impacto,
            NombreRiesgo: "Riesgo de Ransomware en SCADA",
            DescripcionRiesgo: "Impacto sobre operación industrial",
            CreadoPor: "qa.bot");

    private sealed record TestSetup(
        EvaluarRiesgoCommandHandler Handler,
        Mock<ISigeriDbContext> ContextMock,
        List<Riesgo> RiesgosAgregados,
        Guid ActivoId,
        Guid EvaluacionId,
        Guid AmenazaId,
        Guid VulnerabilidadId);
}
