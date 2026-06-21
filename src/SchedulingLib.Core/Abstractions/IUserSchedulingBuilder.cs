using Microsoft.Extensions.DependencyInjection;

namespace SchedulingLib.Core.Abstractions;

/// <summary>
/// Builder returned by <c>AddUserScheduling</c> to configure the user-management domain.
/// Extend this interface in connector packages to add persistence or connector-specific registration methods.
/// </summary>
public interface IUserSchedulingBuilder
{
    /// <summary>Gets the service collection being configured.</summary>
    IServiceCollection Services { get; }
}
