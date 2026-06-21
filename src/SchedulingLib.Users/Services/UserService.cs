using SchedulingLib.Core.Results;
using SchedulingLib.Users.Entities;
using SchedulingLib.Users.Interfaces;
using SchedulingLib.Users.Models;

namespace SchedulingLib.Users.Services;

/// <summary>
/// Default implementation of <see cref="IUserService"/>.
/// </summary>
public sealed class UserService(IUserRepository repository) : IUserService
{
    /// <inheritdoc />
    public async Task<Result<User>> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Fail<User>("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone))
            return Result.Fail<User>("At least one of email or phone must be provided.");

        var user = User.Register(request.Name, request.Email, request.Phone);
        await repository.SaveAsync(user, cancellationToken);
        return Result.Ok(user);
    }

    /// <inheritdoc />
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);
}
