using SchedulingLib.Users.Entities;

namespace SchedulingLib.Users.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_WithEmailOnly_Succeeds()
    {
        var user = new User(Guid.NewGuid(), "Alice", "alice@example.com", null, DateTimeOffset.UtcNow);

        Assert.Equal("alice@example.com", user.Email);
        Assert.Null(user.Phone);
    }

    [Fact]
    public void Constructor_WithPhoneOnly_Succeeds()
    {
        var user = new User(Guid.NewGuid(), "Bob", null, "+1-555-0100", DateTimeOffset.UtcNow);

        Assert.Null(user.Email);
        Assert.Equal("+1-555-0100", user.Phone);
    }

    [Fact]
    public void Constructor_WithBothEmailAndPhone_Succeeds()
    {
        var user = new User(Guid.NewGuid(), "Carol", "carol@example.com", "+1-555-0200", DateTimeOffset.UtcNow);

        Assert.Equal("carol@example.com", user.Email);
        Assert.Equal("+1-555-0200", user.Phone);
    }

    [Fact]
    public void Constructor_WithNoEmailOrPhone_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "Dave", null, null, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithWhitespaceEmailAndPhone_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "Eve", "   ", "   ", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_WithEmptyName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "", "test@example.com", null, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Register_AssignsNewIdAndCurrentTimestamp()
    {
        var before = DateTimeOffset.UtcNow;
        var user = User.Register("Frank", "frank@example.com", null);
        var after = DateTimeOffset.UtcNow;

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.InRange(user.CreatedAt, before, after);
    }

    [Fact]
    public void Constructor_TrimsWhitespaceFromEmail()
    {
        var user = new User(Guid.NewGuid(), "Grace", "  grace@example.com  ", null, DateTimeOffset.UtcNow);

        Assert.Equal("grace@example.com", user.Email);
    }
}
