using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Internal.Dapper;
using SchedulingLib.Persistence.PostgreSQL.Repositories;
using SchedulingLib.Services.Interfaces;

namespace SchedulingLib.Persistence.PostgreSQL;

/// <summary>
/// Extension methods for adding PostgreSQL persistence to the service-scheduling domain.
/// </summary>
public static class PostgreSqlServiceSchedulingBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="PostgreSqlStaffMemberRepository"/>,
    /// <see cref="PostgreSqlServiceAppointmentRepository"/>, and
    /// <see cref="PostgreSqlServiceTypeRepository"/> as the persistence
    /// implementations for the service-scheduling domain.
    /// </summary>
    /// <param name="builder">The service scheduling builder.</param>
    /// <param name="connectionString">A valid Npgsql connection string.</param>
    /// <remarks>
    /// Call <see cref="PostgreSqlSchemaInitializer.InitializeAsync"/> once at startup
    /// to ensure the required tables exist before serving requests.
    /// </remarks>
    public static IServiceSchedulingBuilder AddPostgreSqlPersistence(
        this IServiceSchedulingBuilder builder,
        string connectionString)
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

        builder.Services.AddDbContext<SchedulingDbContext>(opts => opts.UseNpgsql(connectionString));
        builder.Services.AddScoped<IStaffMemberRepository, PostgreSqlStaffMemberRepository>();
        builder.Services.AddScoped<IServiceAppointmentRepository, PostgreSqlServiceAppointmentRepository>();
        builder.Services.AddScoped<IServiceTypeRepository, PostgreSqlServiceTypeRepository>();
        return builder;
    }
}
