using SchedulingLib.Services.Entities;

namespace SchedulingLib.Services.Interfaces;

/// <summary>
/// Persistence contract for <see cref="StaffMember"/> entities.
/// Implement this in your infrastructure layer (EF Core, Dapper, etc.).
/// </summary>
public interface IStaffMemberRepository
{
    /// <summary>Returns the staff member with <paramref name="id"/>, or null if not found.</summary>
    Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new or updated <see cref="StaffMember"/>.</summary>
    Task SaveAsync(StaffMember staffMember, CancellationToken cancellationToken = default);
}
