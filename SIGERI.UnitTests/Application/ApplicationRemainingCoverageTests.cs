using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SIGERI.Application.Common.Behaviors;
using SIGERI.Application.DTOs;
using SIGERI.Application.Activos.Commands;
using SIGERI.Application.Activos.Queries;
using SIGERI.Application.ControlesMadurez.Commands;
using SIGERI.Application.ControlesMadurez.Queries;
using SIGERI.Application.Dependencias.Commands;
using SIGERI.Application.Dependencias.Queries;
using SIGERI.Application.PerfilesCriticos.Commands;
using SIGERI.Application.PerfilesCriticos.Queries;
using SIGERI.Application.Evaluaciones.Commands;
using SIGERI.Application.Evaluaciones.Queries;
using SIGERI.Application.Riesgos.Commands;
using SIGERI.Application.Riesgos.Queries;
using SIGERI.Application.Tratamientos.Commands;
using SIGERI.Application.Tratamientos.Queries;
using SIGERI.Application.Usuarios.Commands;
using SIGERI.Application.Usuarios.Queries;
using SIGERI.Application.Usuarios.Security;
using SIGERI.Domain.Common;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Domain.ValueObjects;
using SIGERI.UnitTests.TestSupport;
using Xunit;

namespace SIGERI.UnitTests.Application;

public sealed class ApplicationRemainingCoverageTests
{
    // ==========================================
    // ACTIVOS
    // ==========================================
    [Fact]
    public async Task EliminarActivo_debe_marcar_como_inactivo()
    {
        var setup = TestDbFactory.Create();
        var handler = new EliminarActivoCommandHandler(setup.Context);

        var id = await handler.Handle(new EliminarActivoCommand(setup.ActivoPrincipal.Id, "editor"), CancellationToken.None);

        id.Should().Be(setup.ActivoPrincipal.Id);
        var activo = await setup.Context.Activos.IgnoreQueryFilters().SingleAsync(x => x.Id == id);
        activo.Estado.Should().BeFalse();
        activo.ActualizadoPor.Should().Be("editor");
    }

    [Fact]
    public async Task EliminarActivo_inexistente_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new EliminarActivoCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(new EliminarActivoCommand(Guid.NewGuid(), "editor"), CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo no existe.");
    }

    // ==========================================
    // EVALUACIONES
    // ==========================================
    [Fact]
    public async Task Handlers_de_evaluaciones_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        // 1. Registrar
        var registrar = new RegistrarEvaluacionCommandHandler(setup.Context);
        var id = await registrar.Handle(
            new RegistrarEvaluacionCommand(DateTime.UtcNow, "Nueva eval", setup.Usuario.Id, setup.ActivoPrincipal.Id, "creador"),
            CancellationToken.None);

        id.Should().NotBeEmpty();

        // 2. Actualizar
        var actualizar = new ActualizarEvaluacionCommandHandler(setup.Context);
        await actualizar.Handle(
            new ActualizarEvaluacionCommand(id, DateTime.UtcNow.AddDays(1), "Eval actualizada", setup.Usuario.Id, setup.ActivoPrincipal.Id, "editor"),
            CancellationToken.None);

        var eval = await setup.Context.Evaluaciones.SingleAsync(e => e.Id == id);
        eval.Observaciones.Should().Be("Eval actualizada");

        // 3. Consultar Todos
        var queryTodos = new ObtenerEvaluacionesQueryHandler(setup.Context);
        var lista = await queryTodos.Handle(new ObtenerEvaluacionesQuery(), CancellationToken.None);
        lista.Should().NotBeEmpty();

        // 4. Consultar Por Id
        var queryPorId = new ObtenerEvaluacionPorIdQueryHandler(setup.Context);
        var eDto = await queryPorId.Handle(new ObtenerEvaluacionPorIdQuery(id), CancellationToken.None);
        eDto.Should().NotBeNull();
        eDto!.Observaciones.Should().Be("Eval actualizada");

        // 5. Eliminar
        var eliminar = new EliminarEvaluacionCommandHandler(setup.Context);
        await eliminar.Handle(new EliminarEvaluacionCommand(id, "editor"), CancellationToken.None);

        eval = await setup.Context.Evaluaciones.IgnoreQueryFilters().SingleAsync(e => e.Id == id);
        eval.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task RegistrarEvaluacion_con_datos_invalidos_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new RegistrarEvaluacionCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new RegistrarEvaluacionCommand(DateTime.UtcNow, "x", Guid.NewGuid(), setup.ActivoPrincipal.Id, "c"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El usuario especificado no existe.");

        await FluentActions.Invoking(() => handler.Handle(
                new RegistrarEvaluacionCommand(DateTime.UtcNow, "x", setup.Usuario.Id, Guid.NewGuid(), "c"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo especificado no existe.");
    }

    [Fact]
    public async Task ActualizarEvaluacion_con_datos_invalidos_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new ActualizarEvaluacionCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarEvaluacionCommand(Guid.NewGuid(), DateTime.UtcNow, "x", setup.Usuario.Id, setup.ActivoPrincipal.Id, "e"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("La evaluación no existe.");

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarEvaluacionCommand(setup.Evaluacion.Id, DateTime.UtcNow, "x", Guid.NewGuid(), setup.ActivoPrincipal.Id, "e"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El usuario especificado no existe.");

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarEvaluacionCommand(setup.Evaluacion.Id, DateTime.UtcNow, "x", setup.Usuario.Id, Guid.NewGuid(), "e"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El activo especificado no existe.");
    }

    // ==========================================
    // RIESGOS
    // ==========================================
    [Fact]
    public async Task Handlers_de_riesgos_deben_consultar_y_actualizar_estado()
    {
        var setup = TestDbFactory.Create();

        // Crear riesgo para testear (usando constructor parameterless para evitar discrepancias de firmas)
        var riesgo = new Riesgo
        {
            Id = Guid.NewGuid(),
            Nombre = "Riesgo Test",
            Descripcion = "Desc Test",
            AmenazaId = setup.Amenaza.Id,
            Amenaza = setup.Amenaza,
            VulnerabilidadId = setup.Vulnerabilidad.Id,
            Vulnerabilidad = setup.Vulnerabilidad,
            Probabilidad = new Probabilidad(3, "Media"),
            Impacto = new Impacto(4, "Alto"),
            PuntajeRiesgo = 12,
            NivelRiesgo = "Alto",
            Estado = EstadoRiesgo.Identificado,
            EvaluacionId = setup.Evaluacion.Id,
            Evaluacion = setup.Evaluacion,
            CreadoPor = "test",
            Activo = true
        };
        setup.Context.Riesgos.Add(riesgo);
        await setup.Context.SaveChangesAsync();

        // 1. Obtener Todos
        var queryTodos = new ObtenerRiesgosQueryHandler(setup.Context);
        (await queryTodos.Handle(new ObtenerRiesgosQuery(), CancellationToken.None)).Should().NotBeEmpty();

        // 2. Obtener Por Id
        var queryPorId = new ObtenerRiesgoPorIdQueryHandler(setup.Context);
        (await queryPorId.Handle(new ObtenerRiesgoPorIdQuery(riesgo.Id), CancellationToken.None)).Should().NotBeNull();
        (await queryPorId.Handle(new ObtenerRiesgoPorIdQuery(Guid.NewGuid()), CancellationToken.None)).Should().BeNull();

        // 3. Obtener Por Evaluacion
        var queryPorEval = new ObtenerRiesgosPorEvaluacionQueryHandler(setup.Context);
        (await queryPorEval.Handle(new ObtenerRiesgosPorEvaluacionQuery(setup.Evaluacion.Id), CancellationToken.None)).Should().NotBeEmpty();

        // 4. Actualizar Estado
        var handlerEstado = new ActualizarEstadoRiesgoCommandHandler(setup.Context);
        await handlerEstado.Handle(new ActualizarEstadoRiesgoCommand(riesgo.Id, EstadoRiesgo.Mitigado, "editor"), CancellationToken.None);

        var rActual = await setup.Context.Riesgos.SingleAsync(r => r.Id == riesgo.Id);
        rActual.Estado.Should().Be(EstadoRiesgo.Mitigado);
    }

    // ==========================================
    // TRATAMIENTOS
    // ==========================================
    [Fact]
    public async Task Handlers_de_tratamientos_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        // Crear un riesgo asociado
        var riesgo = new Riesgo
        {
            Id = Guid.NewGuid(),
            Nombre = "R-1",
            Descripcion = "D",
            AmenazaId = setup.Amenaza.Id,
            Amenaza = setup.Amenaza,
            VulnerabilidadId = setup.Vulnerabilidad.Id,
            Vulnerabilidad = setup.Vulnerabilidad,
            Probabilidad = new Probabilidad(3, "Media"),
            Impacto = new Impacto(3, "Medio"),
            PuntajeRiesgo = 9,
            NivelRiesgo = "Medio",
            Estado = EstadoRiesgo.Identificado,
            EvaluacionId = setup.Evaluacion.Id,
            Evaluacion = setup.Evaluacion,
            CreadoPor = "test",
            Activo = true
        };
        setup.Context.Riesgos.Add(riesgo);
        await setup.Context.SaveChangesAsync();

        // 1. Registrar
        var registrar = new RegistrarTratamientoCommandHandler(setup.Context);
        var id = await registrar.Handle(
            new RegistrarTratamientoCommand(riesgo.Id, "Estrategia", "Mitigación", DateTime.UtcNow, DateTime.UtcNow.AddDays(30), EstadoPlan.Pendiente, 1000.0m, 80.0m, 5000.0m, "creador"),
            CancellationToken.None);

        id.Should().NotBeEmpty();

        // 2. Actualizar
        var actualizar = new ActualizarTratamientoCommandHandler(setup.Context);
        await actualizar.Handle(
            new ActualizarTratamientoCommand(id, "Estrategia Act", "Mitigación Act", DateTime.UtcNow, DateTime.UtcNow.AddDays(40), EstadoPlan.EnProgreso, 1200.0m, 90.0m, 6000.0m, "editor"),
            CancellationToken.None);

        var plan = await setup.Context.PlanesTratamiento.SingleAsync(p => p.Id == id);
        plan.Descripcion.Should().Be("Mitigación Act");

        // 3. Consultar Todos
        var queryTodos = new ObtenerTratamientosQueryHandler(setup.Context);
        (await queryTodos.Handle(new ObtenerTratamientosQuery(), CancellationToken.None)).Should().NotBeEmpty();

        // 4. Consultar Por Id
        var queryPorId = new ObtenerTratamientoPorIdQueryHandler(setup.Context);
        (await queryPorId.Handle(new ObtenerTratamientoPorIdQuery(id), CancellationToken.None)).Should().NotBeNull();
        (await queryPorId.Handle(new ObtenerTratamientoPorIdQuery(Guid.NewGuid()), CancellationToken.None)).Should().BeNull();

        // 5. Consultar Por Riesgo
        var queryPorRiesgo = new ObtenerTratamientosPorRiesgoQueryHandler(setup.Context);
        (await queryPorRiesgo.Handle(new ObtenerTratamientosPorRiesgoQuery(riesgo.Id), CancellationToken.None)).Should().NotBeEmpty();

        // 6. Eliminar
        var eliminar = new EliminarTratamientoCommandHandler(setup.Context);
        await eliminar.Handle(new EliminarTratamientoCommand(id, "editor"), CancellationToken.None);

        plan = await setup.Context.PlanesTratamiento.IgnoreQueryFilters().SingleAsync(p => p.Id == id);
        plan.Activo.Should().BeFalse();
    }

    // ==========================================
    // USUARIOS
    // ==========================================
    [Fact]
    public async Task Handlers_de_usuarios_deben_registrar_actualizar_y_consultar()
    {
        var setup = TestDbFactory.Create();

        // 1. Registrar
        var registrar = new RegistrarUsuarioCommandHandler(setup.Context);
        var id = await registrar.Handle(
            new RegistrarUsuarioCommand("Carlos", "Gómez", "carlos@sigeri.local", "Password123!", RolUsuario.AnalistaRiesgos, "creador"),
            CancellationToken.None);

        id.Should().NotBeEmpty();

        // 2. Actualizar
        var actualizar = new ActualizarUsuarioCommandHandler(setup.Context);
        await actualizar.Handle(
            new ActualizarUsuarioCommand(id, "Carlos Modificado", "Gómez Modificado", "carlos@sigeri.local", RolUsuario.Administrador, true, "editor"),
            CancellationToken.None);

        var usr = await setup.Context.Usuarios.SingleAsync(u => u.Id == id);
        usr.Nombre.Should().Be("Carlos Modificado");

        // 3. Cambiar Password
        var cambiarPass = new CambiarPasswordCommandHandler(setup.Context);
        await cambiarPass.Handle(new CambiarPasswordCommand(id, "NewPassword123!", "editor"), CancellationToken.None);

        // 4. Validar Credenciales
        var validar = new ValidarCredencialesQueryHandler(setup.Context);
        var resValido = await validar.Handle(new ValidarCredencialesQuery("carlos@sigeri.local", "NewPassword123!"), CancellationToken.None);
        resValido.Should().NotBeNull();
        resValido!.Nombre.Should().Be("Carlos Modificado");

        var resInvalido = await validar.Handle(new ValidarCredencialesQuery("carlos@sigeri.local", "WrongPassword"), CancellationToken.None);
        resInvalido.Should().BeNull();

        // 5. Consultar Todos
        var queryTodos = new ObtenerUsuariosQueryHandler(setup.Context);
        (await queryTodos.Handle(new ObtenerUsuariosQuery(), CancellationToken.None)).Should().NotBeEmpty();

        // 6. Consultar Por Id
        var queryPorId = new ObtenerUsuarioPorIdQueryHandler(setup.Context);
        (await queryPorId.Handle(new ObtenerUsuarioPorIdQuery(id), CancellationToken.None)).Should().NotBeNull();
        (await queryPorId.Handle(new ObtenerUsuarioPorIdQuery(Guid.NewGuid()), CancellationToken.None)).Should().BeNull();

        // 7. Eliminar
        var eliminar = new EliminarUsuarioCommandHandler(setup.Context);
        await eliminar.Handle(new EliminarUsuarioCommand(id, "editor"), CancellationToken.None);

        usr = await setup.Context.Usuarios.IgnoreQueryFilters().SingleAsync(u => u.Id == id);
        usr.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task RegistrarUsuario_correo_duplicado_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new RegistrarUsuarioCommandHandler(setup.Context);

        await FluentActions.Invoking(() => handler.Handle(
                new RegistrarUsuarioCommand("Test", "User", "ana@sigeri.local", "Pass!", RolUsuario.AnalistaRiesgos, "c"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Ya existe un usuario con el correo 'ana@sigeri.local'.");
    }

    [Fact]
    public async Task ActualizarUsuario_inexistente_o_correo_duplicado_debe_fallar()
    {
        var setup = TestDbFactory.Create();
        var handler = new ActualizarUsuarioCommandHandler(setup.Context);

        // Inexistente
        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarUsuarioCommand(Guid.NewGuid(), "N", "A", "correo@sigeri.local", RolUsuario.AnalistaRiesgos, true, "e"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El usuario no existe.");

        // Correo duplicado
        var usrNuevo = new Usuario("Carlos", "G", "carlos@sigeri.local", "hash", RolUsuario.AnalistaRiesgos, true, new List<Evaluacion>());
        setup.Context.Usuarios.Add(usrNuevo);
        await setup.Context.SaveChangesAsync();

        await FluentActions.Invoking(() => handler.Handle(
                new ActualizarUsuarioCommand(usrNuevo.Id, "Carlos", "G", "ana@sigeri.local", RolUsuario.AnalistaRiesgos, true, "e"),
                CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("El correo 'ana@sigeri.local' ya está en uso por otro usuario.");
    }

    // ==========================================
    // SECURITY & VALIDATION BEHAVIOR
    // ==========================================
    [Fact]
    public void PasswordHashService_debe_validar_y_verificar_passwords()
    {
        var password = "SuperSecretPassword123!";
        var hash = PasswordHashService.HashPassword(password);

        hash.Should().NotBeNullOrWhiteSpace();
        PasswordHashService.VerifyPassword(password, hash).Should().BeTrue();
        PasswordHashService.VerifyPassword("WrongPassword", hash).Should().BeFalse();
        PasswordHashService.VerifyPassword("", hash).Should().BeFalse();
        PasswordHashService.VerifyPassword(password, "").Should().BeFalse();

        // Legacy SHA256 test (VerifyLegacySha256)
        var sha = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password));
        var legacyHash = Convert.ToHexString(sha);
        PasswordHashService.VerifyPassword(password, legacyHash).Should().BeTrue();
    }

    [Fact]
    public async Task ValidationBehavior_debe_ejecutar_next_o_lanzar_excepcion()
    {
        var validator = new RegistrarActivoCommandValidator();
        var validators = new[] { validator };
        var behavior = new ValidationBehavior<RegistrarActivoCommand, Guid>(validators);

        var validCommand = new RegistrarActivoCommand("COD", "Nombre", "desc", TipoActivo.Software, "p", "u", Criticidad.Media, Guid.NewGuid(), "c");
        var invalidCommand = new RegistrarActivoCommand("", "", "desc", TipoActivo.Software, "p", "u", Criticidad.Media, Guid.Empty, "c");

        // Valid execution
        RequestHandlerDelegate<Guid> delegateRun = (x) => Task.FromResult(Guid.NewGuid());
        var res = await behavior.Handle(validCommand, delegateRun, CancellationToken.None);
        res.Should().NotBeEmpty();

        // Invalid execution (throws ValidationException)
        await FluentActions.Invoking(() => behavior.Handle(invalidCommand, delegateRun, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();

        // Empty validators case
        var emptyBehavior = new ValidationBehavior<RegistrarActivoCommand, Guid>(new List<IValidator<RegistrarActivoCommand>>());
        var resEmpty = await emptyBehavior.Handle(invalidCommand, delegateRun, CancellationToken.None);
        resEmpty.Should().NotBeEmpty();
    }

    // ==========================================
    // VALIDATORS
    // ==========================================
    [Fact]
    public void RegistrarUsuarioCommandValidator_debe_validar_campos()
    {
        var validator = new RegistrarUsuarioCommandValidator();
        validator.Validate(new RegistrarUsuarioCommand("Carlos", "G", "carlos@sigeri.local", "NewPassword123!", RolUsuario.AnalistaRiesgos, "c")).IsValid.Should().BeTrue();
        validator.Validate(new RegistrarUsuarioCommand("", "", "invalid-email", "", RolUsuario.AnalistaRiesgos, "c")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void CambiarPasswordCommandValidator_debe_validar_campos()
    {
        var validator = new CambiarPasswordCommandValidator();
        validator.Validate(new CambiarPasswordCommand(Guid.NewGuid(), "NewPassword123!", "e")).IsValid.Should().BeTrue();
        validator.Validate(new CambiarPasswordCommand(Guid.Empty, "", "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidarCredencialesQueryValidator_debe_validar_campos()
    {
        var validator = new ValidarCredencialesQueryValidator();
        validator.Validate(new ValidarCredencialesQuery("email@sigeri.local", "Password123!")).IsValid.Should().BeTrue();
        validator.Validate(new ValidarCredencialesQuery("", "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void EliminarActivoCommandValidator_debe_validar_campos()
    {
        var validator = new EliminarActivoCommandValidator();
        validator.Validate(new EliminarActivoCommand(Guid.NewGuid(), "user")).IsValid.Should().BeTrue();
        validator.Validate(new EliminarActivoCommand(Guid.Empty, "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ActualizarActivoCommandValidator_debe_validar_campos()
    {
        var validator = new ActualizarActivoCommandValidator();
        validator.Validate(new ActualizarActivoCommand(Guid.NewGuid(), "COD", "N", "D", TipoActivo.Software, "P", "U", Criticidad.Media, Guid.NewGuid(), "user")).IsValid.Should().BeTrue();
        validator.Validate(new ActualizarActivoCommand(Guid.Empty, "", "", "", TipoActivo.Software, "", "", Criticidad.Media, Guid.Empty, "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarControlMadurezCommandValidator_debe_validar_campos()
    {
        var validator = new RegistrarControlMadurezCommandValidator();
        validator.Validate(new RegistrarControlMadurezCommand("CIS-01", "Nombre", FuncionNist.Protect, 2, 4, "desc", "user")).IsValid.Should().BeTrue();
        validator.Validate(new RegistrarControlMadurezCommand("", "", FuncionNist.Protect, -1, 6, "desc", "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ActualizarControlMadurezCommandValidator_debe_validar_campos()
    {
        var validator = new ActualizarControlMadurezCommandValidator();
        validator.Validate(new ActualizarControlMadurezCommand(Guid.NewGuid(), "CIS-01", "Nombre", FuncionNist.Protect, 2, 4, "desc", "user")).IsValid.Should().BeTrue();
        validator.Validate(new ActualizarControlMadurezCommand(Guid.Empty, "", "", FuncionNist.Protect, -1, 6, "desc", "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarDependenciaActivoCommandValidator_debe_validar_campos()
    {
        var validator = new RegistrarDependenciaActivoCommandValidator();
        validator.Validate(new RegistrarDependenciaActivoCommand(Guid.NewGuid(), Guid.NewGuid(), TipoDependenciaActivo.Tecnologica, "desc", 3, "user")).IsValid.Should().BeTrue();
        validator.Validate(new RegistrarDependenciaActivoCommand(Guid.Empty, Guid.Empty, TipoDependenciaActivo.Tecnologica, "", 6, "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ActualizarDependenciaActivoCommandValidator_debe_validar_campos()
    {
        var validator = new ActualizarDependenciaActivoCommandValidator();
        validator.Validate(new ActualizarDependenciaActivoCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), TipoDependenciaActivo.Tecnologica, "desc", 3, "user")).IsValid.Should().BeTrue();
        validator.Validate(new ActualizarDependenciaActivoCommand(Guid.Empty, Guid.Empty, Guid.Empty, TipoDependenciaActivo.Tecnologica, "", 6, "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarPerfilActivoCriticoCommandValidator_debe_validar_campos()
    {
        var validator = new RegistrarPerfilActivoCriticoCommandValidator();
        validator.Validate(new RegistrarPerfilActivoCriticoCommand(Guid.NewGuid(), NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, NivelImpactoOctave.Alto, NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, "t", "f", "h", "user")).IsValid.Should().BeTrue();
        validator.Validate(new RegistrarPerfilActivoCriticoCommand(Guid.Empty, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, "", "", "", "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ActualizarPerfilActivoCriticoCommandValidator_debe_validar_campos()
    {
        var validator = new ActualizarPerfilActivoCriticoCommandValidator();
        validator.Validate(new ActualizarPerfilActivoCriticoCommand(Guid.NewGuid(), Guid.NewGuid(), NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, NivelImpactoOctave.Alto, NivelImpactoOctave.Bajo, NivelImpactoOctave.Medio, "t", "f", "h", "user")).IsValid.Should().BeTrue();
        validator.Validate(new ActualizarPerfilActivoCriticoCommand(Guid.Empty, Guid.Empty, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, NivelImpactoOctave.Bajo, "", "", "", "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarEvaluacionCommandValidator_debe_validar_campos()
    {
        var validator = new RegistrarEvaluacionCommandValidator();
        validator.Validate(new RegistrarEvaluacionCommand(DateTime.UtcNow, "Observaciones", Guid.NewGuid(), Guid.NewGuid(), "user")).IsValid.Should().BeTrue();
        validator.Validate(new RegistrarEvaluacionCommand(default, "", Guid.Empty, Guid.Empty, "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegistrarTratamientoCommandValidator_debe_validar_campos()
    {
        var validator = new RegistrarTratamientoCommandValidator();
        validator.Validate(new RegistrarTratamientoCommand(Guid.NewGuid(), "Estrategia", "Descripcion", DateTime.UtcNow, DateTime.UtcNow.AddDays(5), EstadoPlan.Pendiente, 100.0m, 50.0m, 1000.0m, "user")).IsValid.Should().BeTrue();
        validator.Validate(new RegistrarTratamientoCommand(Guid.Empty, "", "", DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), EstadoPlan.Pendiente, -1.0m, 150.0m, -1.0m, "")).IsValid.Should().BeFalse();
    }

    [Fact]
    public void ActualizarUsuarioCommandValidator_debe_validar_campos()
    {
        var validator = new ActualizarUsuarioCommandValidator();
        validator.Validate(new ActualizarUsuarioCommand(Guid.NewGuid(), "Carlos", "Gómez", "carlos@sigeri.local", RolUsuario.AnalistaRiesgos, true, "user")).IsValid.Should().BeTrue();
        validator.Validate(new ActualizarUsuarioCommand(Guid.Empty, "", "", "invalid-email", RolUsuario.AnalistaRiesgos, true, "")).IsValid.Should().BeFalse();
    }
}
