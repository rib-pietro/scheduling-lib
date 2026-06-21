using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Persistence.PostgreSQL.Repositories;

/// <summary>
/// PostgreSQL-backed implementation of <see cref="IServiceTypeRepository"/>.
/// Queries run through EF Core; upserts and deletes use Dapper for explicit SQL control.
/// </summary>
internal sealed class PostgreSqlServiceTypeRepository(SchedulingDbContext context) : IServiceTypeRepository
{
    /// <inheritdoc />
    public async Task<ServiceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await context.ServiceTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return row is null ? null : ServiceTypeMapper.ToDomain(row);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var rows = await context.ServiceTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows.ConvertAll(ServiceTypeMapper.ToDomain);
    }

    /// <inheritdoc />
    public async Task SaveAsync(ServiceType serviceType, CancellationToken cancellationToken = default)
    {
        var row = ServiceTypeMapper.ToRow(serviceType);

        const string sql = """
            INSERT INTO service_types (id, name, price, duration_ticks)
            VALUES (@Id, @Name, @Price, @DurationTicks)
            ON CONFLICT (id) DO UPDATE SET
                name           = EXCLUDED.name,
                price          = EXCLUDED.price,
                duration_ticks = EXCLUDED.duration_ticks
            """;

        await using var conn = new NpgsqlConnection(context.Database.GetConnectionString());
        await conn.ExecuteAsync(new CommandDefinition(sql, row, cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM service_types WHERE id = @id";
        await using var conn = new NpgsqlConnection(context.Database.GetConnectionString());
        await conn.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
    }
}
