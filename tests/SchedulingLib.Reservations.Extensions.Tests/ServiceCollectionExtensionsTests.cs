using Microsoft.Extensions.DependencyInjection;
using Moq;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Reservations.Extensions;
using SchedulingLib.Reservations.Interfaces;

namespace SchedulingLib.Reservations.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddReservationScheduling_RegistersIReservationService()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => Mock.Of<IReservationRepository>());
        services.AddScoped(_ => Mock.Of<IResourceRepository>());

        services.AddReservationScheduling();

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IReservationService>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddReservationScheduling_WithOptions_RegistersOptions()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => Mock.Of<IReservationRepository>());
        services.AddScoped(_ => Mock.Of<IResourceRepository>());

        services.AddReservationScheduling(o => o = o with { DefaultCurrency = "EUR" });

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<ReservationSchedulingOptions>();
        Assert.NotNull(options);
    }

    [Fact]
    public void AddReservationScheduling_ReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddReservationScheduling();
        Assert.IsAssignableFrom<IReservationSchedulingBuilder>(builder);
    }
}
