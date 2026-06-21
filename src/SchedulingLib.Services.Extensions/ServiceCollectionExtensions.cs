using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.Services;

namespace SchedulingLib.Services.Extensions;

/// <summary>
/// Extension methods for registering the service-scheduling domain with an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all services required for the service-scheduling domain and returns a builder
    /// for further configuration (e.g., adding a calendar connector).
    /// </summary>
    public static IServiceSchedulingBuilder AddServiceScheduling(
        this IServiceCollection services,
        Action<ServiceSchedulingOptions>? configure = null)
    {
        var options = new ServiceSchedulingOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<IServiceAppointmentService, ServiceAppointmentService>();
        services.AddScoped<IServiceTypeService, ServiceTypeService>();

        return new ServiceSchedulingBuilder(services);
    }
}
