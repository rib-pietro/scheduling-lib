using Microsoft.Extensions.DependencyInjection;
using Moq;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Users.Extensions;
using SchedulingLib.Users.Interfaces;

namespace SchedulingLib.Users.Extensions.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddUserScheduling_RegistersIUserService()
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => Mock.Of<IUserRepository>());

        services.AddUserScheduling();

        var provider = services.BuildServiceProvider();
        var service = provider.GetService<IUserService>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddUserScheduling_ReturnsBuilder()
    {
        var services = new ServiceCollection();
        var builder = services.AddUserScheduling();
        Assert.IsAssignableFrom<IUserSchedulingBuilder>(builder);
    }
}
