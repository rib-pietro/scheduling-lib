using SchedulingLib.Core.Results;
using SchedulingLib.Users.Entities;
using SchedulingLib.Users.Models;

namespace SchedulingLib.Users.Interfaces;

/// <summary>
/// Application-level operations for managing platform users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user from <paramref name="request"/>.
    /// Returns a failure result when validation fails (e.g., no contact info supplied).
    /// </summary>
    Task<Result<User>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>Returns the user with <paramref name="id"/>, or <c>null</c> if not found.</summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
