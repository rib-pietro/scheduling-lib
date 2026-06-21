using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Users.Interfaces;
using SchedulingLib.Users.Services;

namespace SchedulingLib.Users.Extensions;

/// <summary>
/// Extension methods for registering the user-management domain with an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IUserService"/> and returns a builder for configuring persistence.
    /// </summary>
    public static IUserSchedulingBuilder AddUserScheduling(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return new UserSchedulingBuilder(services);
    }
}
