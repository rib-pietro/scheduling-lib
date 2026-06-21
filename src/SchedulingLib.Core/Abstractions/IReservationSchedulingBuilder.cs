using Microsoft.Extensions.DependencyInjection;

namespace SchedulingLib.Core.Abstractions;

/// <summary>
/// Builder returned by <c>AddReservationScheduling</c> to configure the reservation-scheduling domain.
/// Extend this interface in connector packages to add connector-specific registration methods.
/// </summary>
public interface IReservationSchedulingBuilder
{
    /// <summary>Gets the service collection being configured.</summary>
    IServiceCollection Services { get; }
}
