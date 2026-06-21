using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Reservations.Interfaces;
using SchedulingLib.Reservations.Services;

namespace SchedulingLib.Reservations.Extensions;

/// <summary>
/// Extension methods for registering the reservation-scheduling domain with an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all services required for the reservation-scheduling domain and returns a builder
    /// for further configuration (e.g., adding a calendar connector).
    /// </summary>
    public static IReservationSchedulingBuilder AddReservationScheduling(
        this IServiceCollection services,
        Action<ReservationSchedulingOptions>? configure = null)
    {
        var options = new ReservationSchedulingOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<IReservationService, ReservationService>();

        return new ReservationSchedulingBuilder(services);
    }
}
