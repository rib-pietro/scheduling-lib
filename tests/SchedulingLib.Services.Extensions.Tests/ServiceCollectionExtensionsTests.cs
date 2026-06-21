using Microsoft.Extensions.DependencyInjection;
using Moq;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Services.Extensions;
using SchedulingLib.Services.Interfaces;

namespace SchedulingLib.Services.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddServiceScheduling_RegistersIServiceAppointmentService()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => Mock.Of<IStaffMemberRepository>());
        services.AddScoped(_ => Mock.Of<IServiceAppointmentRepository>());

        services.AddServiceScheduling();

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IServiceAppointmentService>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddServiceScheduling_WithOptions_RegistersOptions()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => Mock.Of<IStaffMemberRepository>());
        services.AddScoped(_ => Mock.Of<IServiceAppointmentRepository>());

        services.AddServiceScheduling(o => o = o with { DefaultTimeZoneId = "Europe/Lisbon" });

        var provider = services.BuildServiceProvider();
        var options = provider.GetService<ServiceSchedulingOptions>();
        Assert.NotNull(options);
    }

    [Fact]
    public void AddServiceScheduling_ReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddServiceScheduling();
        Assert.IsAssignableFrom<IServiceSchedulingBuilder>(builder);
    }
}
