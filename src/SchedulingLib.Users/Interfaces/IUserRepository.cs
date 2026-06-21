using SchedulingLib.Users.Entities;

namespace SchedulingLib.Users.Interfaces;

/// <summary>
/// Persistence contract for <see cref="User"/> aggregates.
/// </summary>
public interface IUserRepository
{
    /// <summary>Returns the user with <paramref name="id"/>, or <c>null</c> if not found.</summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Inserts or updates the user.</summary>
    Task SaveAsync(User user, CancellationToken cancellationToken = default);
}
