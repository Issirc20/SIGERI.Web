using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SIGERI.Application;
using SIGERI.Application.Activos.Commands;
using SIGERI.Application.Activos.Queries;
using SIGERI.Application.ControlesMadurez.Commands;
using SIGERI.Application.ControlesMadurez.Queries;
using SIGERI.Application.DTOs;
using SIGERI.Application.Dependencias.Commands;
using SIGERI.Application.Dependencias.Queries;
using SIGERI.Application.PerfilesCriticos.Commands;
using SIGERI.Application.PerfilesCriticos.Queries;
using SIGERI.Domain.Common;
using SIGERI.Domain.Enums;
using SIGERI.UnitTests.TestSupport;
using Xunit;

namespace SIGERI.UnitTests.Application;

public sealed class ApplicationCoverageTests
{
    [Fact]
    public async Task Handlers_de_activos_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        var registrar = new RegistrarActivoCommandHandler(setup.Context);
        var nuevoId = await registrar.Handle(
            new RegistrarActivoCommand("  ACT-200  ", "  Nuevo Activo  ", "  Desc  ", TipoActivo.Hardware, "  Owner  ", "  Lima  ", Criticidad.Media, setup.Organizacion.Id, "  test  "),
            CancellationToken.None);

        nuevoId.Should().NotBeEmpty();
        setup.Context.Activos.Single(x => x.Id == nuevoId).Codigo.Should().Be("ACT-200");

        var actualizar = new ActualizarActivoCommandHandler(setup.Context);
        var actualizadoId = await actualizar.Handle(
            new ActualizarActivoCommand(setup.ActivoPrincipal.Id, " ACT-001X ", " SCADA X ", " Desc X ", TipoActivo.Software, " Nuevo ", " Planta Sur ", Criticidad.Alta, setup.Organizacion.Id, " editor "),
            CancellationToken.None);

        actualizadoId.Should().Be(setup.ActivoPrincipal.Id);
        setup.Context.Activos.Single(x => x.Id == setup.ActivoPrincipal.Id).Nombre.Should().Be("SCADA X");

        var obtenerTodos = new ObtenerActivosQueryHandler(setup.Context);
        var activos = await obtenerTodos.Handle(new ObtenerActivosQuery(), CancellationToken.None);
        activos.Should().NotBeEmpty();

        var obtenerPorId = new ObtenerActivoPorIdQueryHandler(setup.Context);
        var activo = await obtenerPorId.Handle(new ObtenerActivoPorIdQuery(setup.ActivoPrincipal.Id), CancellationToken.None);
        activo.Should().NotBeNull();

        var inexistente = await obtenerPorId.Handle(new ObtenerActivoPorIdQuery(Guid.NewGuid()), CancellationToken.None);
        inexistente.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarActivo_si_no_existe_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new ActualizarActivoCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarActivoCommand(Guid.NewGuid(), "A", "B", "C", TipoActivo.Hardware, "P", "U", Criticidad.Baja, setup.Organizacion.Id, "test"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo no existe.");
    }

    [Fact]
    public async Task Handlers_de_dependencias_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        var registrar = new RegistrarDependenciaActivoCommandHandler(setup.Context);
        var id = await registrar.Handle(
            new RegistrarDependenciaActivoCommand(setup.ActivoPrincipal.Id, setup.ActivoSecundario.Id, TipoDependenciaActivo.Energetica, "  Nueva dependencia ", 5, "  creador "),
            CancellationToken.None);

        id.Should().NotBeEmpty();

        var actualizar = new ActualizarDependenciaActivoCommandHandler(setup.Context);
        await actualizar.Handle(
            new ActualizarDependenciaActivoCommand(setup.Dependencia.Id, setup.ActivoSecundario.Id, setup.ActivoPrincipal.Id, TipoDependenciaActivo.Fisica, "  Actualizada ", 3, "  editor "),
            CancellationToken.None);

        setup.Context.DependenciasActivos.Single(x => x.Id == setup.Dependencia.Id).Descripcion.Should().Be("Actualizada");

        var listar = new ObtenerDependenciasActivoQueryHandler(setup.Context);
        var dependencias = await listar.Handle(new ObtenerDependenciasActivoQuery(), CancellationToken.None);
        dependencias.Should().NotBeEmpty();

        var porId = new ObtenerDependenciaActivoPorIdQueryHandler(setup.Context);
        (await porId.Handle(new ObtenerDependenciaActivoPorIdQuery(setup.Dependencia.Id), CancellationToken.None)).Should().NotBeNull();
        (await porId.Handle(new ObtenerDependenciaActivoPorIdQuery(Guid.NewGuid()), CancellationToken.None)).Should().BeNull();
    }

    [Fact]
    public async Task RegistrarDependencia_con_datos_invalidos_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new RegistrarDependenciaActivoCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new RegistrarDependenciaActivoCommand(Guid.NewGuid(), setup.ActivoSecundario.Id, TipoDependenciaActivo.Tecnologica, "x", 3, "test"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo origen no existe.");

        await FluentActions.Invoking(() => handler.Handle(
                new RegistrarDependenciaActivoCommand(setup.ActivoPrincipal.Id, setup.ActivoSecundario.Id, TipoDependenciaActivo.Tecnologica, "x", 7, "test"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La criticidad operativa debe estar entre 1 y 5.");
    }

    [Fact]
    public async Task ActualizarDependencia_si_no_existe_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new ActualizarDependenciaActivoCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarDependenciaActivoCommand(Guid.NewGuid(), setup.ActivoPrincipal.Id, setup.ActivoSecundario.Id, TipoDependenciaActivo.Tecnologica, "desc", 2, "test"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La dependencia no existe.");
    }

    [Fact]
    public async Task Handlers_de_controles_madurez_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        var registrar = new RegistrarControlMadurezCommandHandler(setup.Context);
        var id = await registrar.Handle(
            new RegistrarControlMadurezCommand("  CIS-99 ", "  Control X ", FuncionNist.Govern, 1, 4, "  desc ", "  creador "),
            CancellationToken.None);

        id.Should().NotBeEmpty();

        var actualizar = new ActualizarControlMadurezCommandHandler(setup.Context);
        await actualizar.Handle(
            new ActualizarControlMadurezCommand(setup.ControlMadurez.Id, " CIS-01 ", " Inventario actualizado ", FuncionNist.Protect, 3, 4, "  detalle ", " editor "),
            CancellationToken.None);

        setup.Context.ControlesMadurez.Single(x => x.Id == setup.ControlMadurez.Id).Nombre.Should().Be("Inventario actualizado");

        var listar = new ObtenerControlesMadurezQueryHandler(setup.Context);
        (await listar.Handle(new ObtenerControlesMadurezQuery(), CancellationToken.None)).Should().NotBeEmpty();

        var porId = new ObtenerControlMadurezPorIdQueryHandler(setup.Context);
        (await porId.Handle(new ObtenerControlMadurezPorIdQuery(setup.ControlMadurez.Id), CancellationToken.None)).Should().NotBeNull();
        (await porId.Handle(new ObtenerControlMadurezPorIdQuery(Guid.NewGuid()), CancellationToken.None)).Should().BeNull();
    }

    [Fact]
    public async Task ActualizarControlMadurez_si_no_existe_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new ActualizarControlMadurezCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarControlMadurezCommand(Guid.NewGuid(), "C", "N", FuncionNist.Detect, 1, 2, "D", "U"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El control de madurez no existe.");
    }

    [Fact]
    public async Task Handlers_de_perfiles_criticos_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        var registrar = new RegistrarPerfilActivoCriticoCommandHandler(setup.Context);
        var id = await registrar.Handle(
            new RegistrarPerfilActivoCriticoCommand(setup.ActivoPrincipal.Id, NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, NivelImpactoOctave.Alto, NivelImpactoOctave.Medio, NivelImpactoOctave.Bajo, " tec ", " fis ", " hum ", " creador "),
            CancellationToken.None);

        id.Should().NotBeEmpty();

        var actualizar = new ActualizarPerfilActivoCriticoCommandHandler(setup.Context);
        await actualizar.Handle(
            new ActualizarPerfilActivoCriticoCommand(setup.Perfil.Id, setup.ActivoSecundario.Id, NivelImpactoOctave.Alto, NivelImpactoOctave.Alto, NivelImpactoOctave.Medio, NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, " tec2 ", " fis2 ", " hum2 ", " editor "),
            CancellationToken.None);

        setup.Context.PerfilesActivosCriticos.Single(x => x.Id == setup.Perfil.Id).ContenedoresTecnicos.Should().Be("tec2");

        var listar = new ObtenerPerfilesActivoCriticoQueryHandler(setup.Context);
        (await listar.Handle(new ObtenerPerfilesActivoCriticoQuery(), CancellationToken.None)).Should().NotBeEmpty();

        var porId = new ObtenerPerfilActivoCriticoPorIdQueryHandler(setup.Context);
        (await porId.Handle(new ObtenerPerfilActivoCriticoPorIdQuery(setup.Perfil.Id), CancellationToken.None)).Should().NotBeNull();
        (await porId.Handle(new ObtenerPerfilActivoCriticoPorIdQuery(Guid.NewGuid()), CancellationToken.None)).Should().BeNull();
    }

    [Fact]
    public async Task Handlers_perfiles_si_no_existe_debe_fallar()
    {
        var setup = TestDbFactory.Create();

        var registrar = new RegistrarPerfilActivoCriticoCommandHandler(setup.Context);
        await FluentActions.Invoking(() => registrar.Handle(
                new RegistrarPerfilActivoCriticoCommand(Guid.NewGuid(), NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, "t", "f", "h", "u"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo especificado no existe.");

        var actualizar = new ActualizarPerfilActivoCriticoCommandHandler(setup.Context);
        await FluentActions.Invoking(() => actualizar.Handle(
                new ActualizarPerfilActivoCriticoCommand(Guid.NewGuid(), setup.ActivoPrincipal.Id, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, "t", "f", "h", "u"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El perfil crítico no existe.");
    }

    [Fact]
    public void RegistrarActivoCommandValidator_debe_validar_campos_requeridos()
    {
        var validator = new RegistrarActivoCommandValidator();
        var invalido = new RegistrarActivoCommand("", "", "desc", TipoActivo.Software, "p", "u", Criticidad.Media, Guid.Empty, "c");
        var valido = new RegistrarActivoCommand("COD", "Nombre", "desc", TipoActivo.Software, "p", "u", Criticidad.Media, Guid.NewGuid(), "c");

        validator.Validate(invalido).IsValid.Should().BeFalse();
        validator.Validate(valido).IsValid.Should().BeTrue();
    }

    [Fact]
    public void DependencyInjection_y_dtos_deben_resolverse_correctamente()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        services.AddLogging();

        using var provider = services.BuildServiceProvider();

        provider.GetService<IMediator>().Should().NotBeNull();
        provider.GetService<IValidator<RegistrarActivoCommand>>().Should().NotBeNull();

        var activoDto = new ActivoDto(Guid.NewGuid(), "A", "N", "D", TipoActivo.Hardware, "P", "U", Criticidad.Baja, Guid.NewGuid(), "ORG", true);
        var controlDto = new ControlMadurezDto(Guid.NewGuid(), "C", "N", FuncionNist.Recover, 1, 2, "D");
        var dependenciaDto = new DependenciaActivoDto(Guid.NewGuid(), Guid.NewGuid(), "O", Guid.NewGuid(), "D", TipoDependenciaActivo.Tecnologica, "desc", 2);
        var perfilDto = new PerfilActivoCriticoDto(Guid.NewGuid(), Guid.NewGuid(), "A", NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, NivelImpactoOctave.Alto, NivelImpactoOctave.Bajo, NivelImpactoOctave.Alto, "t", "f", "h");

        activoDto.Codigo.Should().Be("A");
        controlDto.Funcion.Should().Be(FuncionNist.Recover);
        dependenciaDto.CriticidadOperativa.Should().Be(2);
        perfilDto.ContenedoresHumanos.Should().Be("h");
    }
}
