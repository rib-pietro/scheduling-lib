using SchedulingLib.Core.Results;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Interfaces;

/// <summary>
/// Manages the catalog of service types that staff members can offer.
/// </summary>
public interface IServiceTypeService
{
    /// <summary>
    /// Creates a new service type with <paramref name="name"/>, <paramref name="price"/>, and <paramref name="duration"/>.
    /// </summary>
    Task<Result<ServiceType>> CreateAsync(string name, decimal price, TimeSpan duration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently removes the service type with <paramref name="id"/>.
    /// Returns a failure result when no matching service type exists.
    /// </summary>
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns all service types in the catalog.</summary>
    Task<Result<IReadOnlyList<ServiceType>>> GetAllAsync(CancellationToken cancellationToken = default);
}
