using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;

namespace SchedulingLib.Users.Extensions;

internal sealed class UserSchedulingBuilder(IServiceCollection services) : IUserSchedulingBuilder
{
    public IServiceCollection Services { get; } = services;
}
