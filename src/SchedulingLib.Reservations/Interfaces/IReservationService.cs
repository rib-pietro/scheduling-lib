using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Reservations.Entities;
using SchedulingLib.Reservations.Models;

namespace SchedulingLib.Reservations.Interfaces;

/// <summary>
/// Orchestrates creation, cancellation, and availability queries for reservations.
/// </summary>
public interface IReservationService
{
    /// <summary>
    /// Creates a reservation. Validates the check-in day against the resource's availability window
    /// and checks for overlapping bookings before confirming.
    /// </summary>
    Task<Result<Reservation>> ReserveAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels the reservation with <paramref name="reservationId"/>.
    /// Removes the event from the connected calendar if one is configured.
    /// </summary>
    Task<Result<bool>> CancelAsync(Guid reservationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true if <paramref name="resourceId"/> is available for the entire <paramref name="range"/>.
    /// </summary>
    Task<Result<bool>> IsAvailableAsync(Guid resourceId, DateRange range, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the date ranges during which <paramref name="resourceId"/> is unavailable in the given month.
    /// </summary>
    Task<Result<IReadOnlyList<DateRange>>> GetUnavailableDatesAsync(Guid resourceId, int year, int month, CancellationToken cancellationToken = default);
}
