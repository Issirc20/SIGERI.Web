using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;
using FluentAssertions;
using SIGERI.Infrastructure.Persistence;
using SIGERI.Domain.Entities;
using SIGERI.Domain.Enums;
using SIGERI.Application.Activos.Commands;
using SIGERI.Application.Activos.Queries;

[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace SIGERI.IntegrationTests;

public class SigeriIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private SigeriDbContext _context = null!;

    public async Task InitializeAsync()
    {
        // 1. Start the container
        await _dbContainer.StartAsync();

        // 2. Set up db context options using container connection string
        var options = new DbContextOptionsBuilder<SigeriDbContext>()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;

        _context = new SigeriDbContext(options);

        // 3. Migrate database to create tables and schemas
        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // 4. Dispose DbContext and stop the container
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
        await _dbContainer.StopAsync();
    }

    [Fact]
    public async Task Debe_crear_y_consultar_un_activo_en_base_de_datos_real_de_sql_server()
    {
        // Arrange
        var organizacion = new Organizacion 
        { 
            Id = Guid.NewGuid(),
            Nombre = "Organizacion Test de Integracion", 
            Siglas = "OTI", 
            Activo = true 
        };
        _context.Organizaciones.Add(organizacion);
        await _context.SaveChangesAsync();

        var handlerRegistrar = new RegistrarActivoCommandHandler(_context);
        var command = new RegistrarActivoCommand(
            Codigo: "ACT-INT-001",
            Nombre: "Activo de Integracion Testcontainers",
            Descripcion: "Prueba de Integracion real con MS SQL Server en Docker",
            TipoActivo: TipoActivo.Hardware,
            Propietario: "Unidad de Ciberseguridad",
            Ubicacion: "Sala de Servidores Principal",
            Criticidad: Criticidad.Alta,
            OrganizacionId: organizacion.Id,
            CreadoPor: "integration-tests-agent"
        );

        // Act
        var id = await handlerRegistrar.Handle(command, CancellationToken.None);

        // Assert
        id.Should().NotBeEmpty();

        var queryHandler = new ObtenerActivoPorIdQueryHandler(_context);
        var result = await queryHandler.Handle(new ObtenerActivoPorIdQuery(id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Nombre.Should().Be("Activo de Integracion Testcontainers");
        result.Codigo.Should().Be("ACT-INT-001");
        result.Criticidad.Should().Be(Criticidad.Alta);
    }

    [Fact]
    public async Task Debe_eliminar_activo_marcandolo_como_inactivo_en_base_de_datos_real_de_sql_server()
    {
        // Arrange
        var organizacion = new Organizacion 
        { 
            Id = Guid.NewGuid(),
            Nombre = "Organizacion Test de Borrado", 
            Siglas = "OTB", 
            Activo = true 
        };
        _context.Organizaciones.Add(organizacion);
        await _context.SaveChangesAsync();

        var handlerRegistrar = new RegistrarActivoCommandHandler(_context);
        var id = await handlerRegistrar.Handle(
            new RegistrarActivoCommand(
                Codigo: "ACT-INT-002", 
                Nombre: "Activo de Integracion Temporal", 
                Descripcion: "Prueba de borrado lógico", 
                TipoActivo: TipoActivo.Software, 
                Propietario: "Sistemas", 
                Ubicacion: "Nube Privada", 
                Criticidad: Criticidad.Media, 
                OrganizacionId: organizacion.Id, 
                CreadoPor: "integration-tests-agent"
            ), CancellationToken.None);

        var handlerEliminar = new EliminarActivoCommandHandler(_context);

        // Act
        await handlerEliminar.Handle(new EliminarActivoCommand(id, "editor-de-pruebas"), CancellationToken.None);

        // Assert
        var queryHandler = new ObtenerActivoPorIdQueryHandler(_context);
        var result = await queryHandler.Handle(new ObtenerActivoPorIdQuery(id), CancellationToken.None);
        
        result.Should().NotBeNull();
        result!.Estado.Should().BeFalse(); // El soft delete setea el estado del negocio a false

        var dbActivo = await _context.Activos.IgnoreQueryFilters().SingleAsync(x => x.Id == id);
        dbActivo.Estado.Should().BeFalse();
        dbActivo.ActualizadoPor.Should().Be("editor-de-pruebas");
    }
}
