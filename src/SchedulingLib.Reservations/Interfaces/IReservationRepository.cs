using SchedulingLib.Core.Primitives;
using SchedulingLib.Reservations.Entities;

namespace SchedulingLib.Reservations.Interfaces;

/// <summary>
/// Persistence contract for <see cref="Reservation"/> entities.
/// Implement this in your infrastructure layer (EF Core, Dapper, etc.).
/// </summary>
public interface IReservationRepository
{
    /// <summary>Returns the reservation with <paramref name="id"/>, or null if not found.</summary>
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new or updated <see cref="Reservation"/>.</summary>
    Task SaveAsync(Reservation reservation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all non-cancelled reservations for <paramref name="resourceId"/> whose date range
    /// overlaps with <paramref name="range"/>.
    /// </summary>
    Task<IReadOnlyList<Reservation>> GetOverlappingAsync(Guid resourceId, DateRange range, CancellationToken cancellationToken = default);
}
