using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Interfaces;

/// <summary>
/// Persistence contract for <see cref="ServiceType"/> catalog entries.
/// Implement this in your infrastructure layer.
/// </summary>
public interface IServiceTypeRepository
{
    /// <summary>Returns the service type with <paramref name="id"/>, or null if not found.</summary>
    Task<ServiceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns all service types in the catalog.</summary>
    Task<IReadOnlyList<ServiceType>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Persists a new or updated <see cref="ServiceType"/>.</summary>
    Task SaveAsync(ServiceType serviceType, CancellationToken cancellationToken = default);

    /// <summary>Permanently removes the service type with <paramref name="id"/>.</summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
