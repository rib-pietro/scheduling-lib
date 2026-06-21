namespace SchedulingLib.Users.Models;

/// <summary>
/// Input for registering a new user on the platform.
/// At least one of <see cref="Email"/> or <see cref="Phone"/> must be supplied.
/// </summary>
public record RegisterUserRequest(string Name, string? Email, string? Phone);
