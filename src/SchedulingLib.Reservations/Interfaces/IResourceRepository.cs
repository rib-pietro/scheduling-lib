using SchedulingLib.Reservations.ValueObjects;

namespace SchedulingLib.Reservations.Interfaces;

/// <summary>
/// Persistence contract for <see cref="ReservableResource"/> records.
/// Implement this in your infrastructure layer (EF Core, Dapper, etc.).
/// </summary>
public interface IResourceRepository
{
    /// <summary>Returns the resource with <paramref name="id"/>, or null if not found.</summary>
    Task<ReservableResource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new or updated <see cref="ReservableResource"/>.</summary>
    Task SaveAsync(ReservableResource resource, CancellationToken cancellationToken = default);
}
