using Microsoft.Extensions.DependencyInjection;

namespace SchedulingLib.Core.Abstractions;

/// <summary>
/// Builder returned by <c>AddServiceScheduling</c> to configure the service-scheduling domain.
/// Extend this interface in connector packages to add connector-specific registration methods.
/// </summary>
public interface IServiceSchedulingBuilder
{
    /// <summary>Gets the service collection being configured.</summary>
    IServiceCollection Services { get; }
}
