using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Repositories;
using SchedulingLib.Services.Interfaces;
using Testcontainers.PostgreSql;

namespace SchedulingLib.Persistence.PostgreSQL.Tests;

public sealed class PostgreSqlDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private ServiceProvider _serviceProvider = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        var connectionString = _container.GetConnectionString();
        await PostgreSqlSchemaInitializer.InitializeAsync(connectionString);

        var services = new ServiceCollection();
        services.AddDbContext<SchedulingDbContext>(opts => opts.UseNpgsql(connectionString));
        services.AddScoped<IStaffMemberRepository, PostgreSqlStaffMemberRepository>();
        services.AddScoped<IServiceAppointmentRepository, PostgreSqlServiceAppointmentRepository>();
        services.AddScoped<IServiceTypeRepository, PostgreSqlServiceTypeRepository>();
        _serviceProvider = services.BuildServiceProvider();
    }

    public IServiceScope CreateScope() => _serviceProvider.CreateScope();

    public async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
        await _container.DisposeAsync();
    }
}

[CollectionDefinition("PostgreSQL")]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlDatabaseFixture>;
