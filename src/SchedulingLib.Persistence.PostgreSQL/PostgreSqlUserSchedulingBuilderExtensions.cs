using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Repositories;
using SchedulingLib.Users.Interfaces;

namespace SchedulingLib.Persistence.PostgreSQL;

/// <summary>
/// Extension methods for adding PostgreSQL persistence to the user-management domain.
/// </summary>
public static class PostgreSqlUserSchedulingBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="PostgreSqlUserRepository"/> as the persistence implementation
    /// for the user-management domain.
    /// </summary>
    /// <param name="builder">The user scheduling builder.</param>
    /// <param name="connectionString">A valid Npgsql connection string.</param>
    /// <remarks>
    /// Call <see cref="PostgreSqlSchemaInitializer.InitializeAsync"/> once at startup
    /// to ensure the required tables exist before serving requests.
    /// </remarks>
    public static IUserSchedulingBuilder AddPostgreSqlPersistence(
        this IUserSchedulingBuilder builder,
        string connectionString)
    {
        builder.Services.AddDbContext<SchedulingDbContext>(opts => opts.UseNpgsql(connectionString));
        builder.Services.AddScoped<IUserRepository, PostgreSqlUserRepository>();
        return builder;
    }
}
