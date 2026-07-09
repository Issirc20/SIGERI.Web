using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SIGERI.Application.Interfaces;
using SIGERI.Infrastructure;
using SIGERI.Infrastructure.Persistence;
using Xunit;

namespace SIGERI.UnitTests.Infrastructure;

public sealed class InfrastructureCoverageTests
{
    [Fact]
    public void SeedData_debe_poblar_informacion_y_evitar_duplicados()
    {
        var options = new DbContextOptionsBuilder<SigeriDbContext>()
            .UseInMemoryDatabase($"SIGERI_Seed_{Guid.NewGuid():N}")
            .Options;

        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["DevCredentials:AdminCorreo"]).Returns("admin@sigeri.local");
        configurationMock.Setup(x => x["DevCredentials:AdminPassword"]).Returns("Admin123!");
        configurationMock.Setup(x => x["DevCredentials:AnalistaCorreo"]).Returns("analista@sigeri.local");
        configurationMock.Setup(x => x["DevCredentials:AnalistaPassword"]).Returns("Analista123!");

        using var context = new SigeriDbContext(options);

        SeedData.EnsureSeeded(context, configurationMock.Object);

        context.Organizaciones.Count().Should().BeGreaterThan(0);
        context.Activos.Count().Should().BeGreaterThan(0);
        context.DependenciasActivos.Count().Should().BeGreaterThan(0);
        context.PerfilesActivosCriticos.Count().Should().BeGreaterThan(0);
        context.ControlesMadurez.Count().Should().BeGreaterThan(0);

        var orgCount = context.Organizaciones.Count();
        var activosCount = context.Activos.Count();

        SeedData.EnsureSeeded(context, configurationMock.Object);

        context.Organizaciones.Count().Should().Be(orgCount);
        context.Activos.Count().Should().Be(activosCount);
    }

    [Fact]
    public void AddInfrastructureServices_debe_registrar_dependencias()
    {
        var connectionSectionMock = new Mock<IConfigurationSection>();
        connectionSectionMock.Setup(x => x["DefaultConnection"]).Returns("Server=(localdb)\\MSSQLLocalDB;Database=SIGERI_Test;Trusted_Connection=True;");

        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x.GetSection("ConnectionStrings")).Returns(connectionSectionMock.Object);

        var configuration = configurationMock.Object;

        var services = new ServiceCollection();
        services.AddInfrastructureServices(configuration);

        using var provider = services.BuildServiceProvider();

        provider.GetService<SigeriDbContext>().Should().NotBeNull();
        provider.GetService<ISigeriDbContext>().Should().NotBeNull();
    }
}
