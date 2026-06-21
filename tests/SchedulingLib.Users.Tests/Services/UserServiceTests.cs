using Moq;
using SchedulingLib.Users.Entities;
using SchedulingLib.Users.Interfaces;
using SchedulingLib.Users.Models;
using SchedulingLib.Users.Services;

namespace SchedulingLib.Users.Tests.Services;

public class UserServiceTests
{
    private static UserService CreateSut(out Mock<IUserRepository> repo)
    {
        repo = new Mock<IUserRepository>();
        return new UserService(repo.Object);
    }

    [Fact]
    public async Task RegisterAsync_EmailOnly_Succeeds()
    {
        var sut = CreateSut(out var repo);
        var request = new RegisterUserRequest("Alice", "alice@example.com", null);

        var result = await sut.RegisterAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Alice", result.Value.Name);
        Assert.Equal("alice@example.com", result.Value.Email);
        Assert.Null(result.Value.Phone);
        repo.Verify(r => r.SaveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_PhoneOnly_Succeeds()
    {
        var sut = CreateSut(out var repo);
        var request = new RegisterUserRequest("Bob", null, "+1-555-0100");

        var result = await sut.RegisterAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("+1-555-0100", result.Value!.Phone);
        Assert.Null(result.Value.Email);
    }

    [Fact]
    public async Task RegisterAsync_BothEmailAndPhone_Succeeds()
    {
        var sut = CreateSut(out var repo);
        var request = new RegisterUserRequest("Carol", "carol@example.com", "+1-555-0200");

        var result = await sut.RegisterAsync(request);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RegisterAsync_NoEmailOrPhone_Fails()
    {
        var sut = CreateSut(out var repo);
        var request = new RegisterUserRequest("Dave", null, null);

        var result = await sut.RegisterAsync(request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        repo.Verify(r => r.SaveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_EmptyName_Fails()
    {
        var sut = CreateSut(out var repo);
        var request = new RegisterUserRequest("", "test@example.com", null);

        var result = await sut.RegisterAsync(request);

        Assert.False(result.IsSuccess);
        repo.Verify(r => r.SaveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var sut = CreateSut(out var repo);
        var id = Guid.NewGuid();
        var user = new User(id, "Eve", "eve@example.com", null, DateTimeOffset.UtcNow);
        repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var found = await sut.GetByIdAsync(id);

        Assert.Same(user, found);
    }

    [Fact]
    public async Task GetByIdAsync_UnknownId_ReturnsNull()
    {
        var sut = CreateSut(out var repo);
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var found = await sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(found);
    }
}
