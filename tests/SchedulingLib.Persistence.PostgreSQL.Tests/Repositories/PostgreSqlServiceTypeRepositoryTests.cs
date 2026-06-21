using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Persistence.PostgreSQL.Tests.Repositories;

[Collection("PostgreSQL")]
public sealed class PostgreSqlServiceTypeRepositoryTests(PostgreSqlDatabaseFixture fixture)
{
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_ThenGetById_RoundTripsAllFields()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var serviceType = new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(60));
        await repo.SaveAsync(serviceType);
        var retrieved = await repo.GetByIdAsync(serviceType.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(serviceType.Id, retrieved.Id);
        Assert.Equal("Haircut", retrieved.Name);
        Assert.Equal(25.00m, retrieved.Price);
        Assert.Equal(TimeSpan.FromMinutes(60), retrieved.Duration);
    }

    [Fact]
    public async Task SaveAsync_CalledTwice_UpdatesExistingRecord()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var id = Guid.NewGuid();
        await repo.SaveAsync(new ServiceType(id, "Haircut", 25.00m, TimeSpan.FromMinutes(60)));
        await repo.SaveAsync(new ServiceType(id, "Premium Haircut", 40.00m, TimeSpan.FromMinutes(90)));

        var retrieved = await repo.GetByIdAsync(id);
        Assert.NotNull(retrieved);
        Assert.Equal("Premium Haircut", retrieved.Name);
        Assert.Equal(40.00m, retrieved.Price);
        Assert.Equal(TimeSpan.FromMinutes(90), retrieved.Duration);
    }

    [Fact]
    public async Task GetAllAsync_ContainsAllSaved()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        await repo.SaveAsync(new ServiceType(id1, "ServiceA", 10.00m, TimeSpan.FromMinutes(30)));
        await repo.SaveAsync(new ServiceType(id2, "ServiceB", 20.00m, TimeSpan.FromMinutes(45)));

        var all = await repo.GetAllAsync();

        Assert.Contains(all, s => s.Id == id1);
        Assert.Contains(all, s => s.Id == id2);
    }

    [Fact]
    public async Task DeleteAsync_RemovesRecord()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var serviceType = new ServiceType(Guid.NewGuid(), "Temporary", 5.00m, TimeSpan.FromMinutes(15));
        await repo.SaveAsync(serviceType);

        await repo.DeleteAsync(serviceType.Id);

        var retrieved = await repo.GetByIdAsync(serviceType.Id);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_DoesNotThrow()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        await repo.DeleteAsync(Guid.NewGuid());
    }
}
