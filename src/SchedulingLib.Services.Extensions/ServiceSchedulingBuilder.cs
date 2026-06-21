using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;

namespace SchedulingLib.Services.Extensions;

/// <summary>
/// Default implementation of <see cref="IServiceSchedulingBuilder"/>.
/// </summary>
internal sealed class ServiceSchedulingBuilder : IServiceSchedulingBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; }

    internal ServiceSchedulingBuilder(IServiceCollection services) => Services = services;
}
