using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;

namespace SchedulingLib.Reservations.Extensions;

/// <summary>
/// Default implementation of <see cref="IReservationSchedulingBuilder"/>.
/// </summary>
internal sealed class ReservationSchedulingBuilder : IReservationSchedulingBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; }

    internal ReservationSchedulingBuilder(IServiceCollection services) => Services = services;
}
