using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;
using SchedulingLib.Users.Entities;
using SchedulingLib.Users.Interfaces;

namespace SchedulingLib.Persistence.PostgreSQL.Repositories;

/// <summary>
/// PostgreSQL-backed implementation of <see cref="IUserRepository"/>.
/// Queries run through EF Core; upserts use Dapper for explicit SQL control.
/// </summary>
internal sealed class PostgreSqlUserRepository(SchedulingDbContext context) : IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return row is null ? null : UserMapper.ToDomain(row);
    }

    /// <inheritdoc />
    public async Task SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        var row = UserMapper.ToRow(user);

        const string sql = """
            INSERT INTO users (id, name, email, phone, created_at)
            VALUES (@Id, @Name, @Email, @Phone, @CreatedAt)
            ON CONFLICT (id) DO UPDATE SET
                name  = EXCLUDED.name,
                email = EXCLUDED.email,
                phone = EXCLUDED.phone
            """;

        await using var conn = new NpgsqlConnection(context.Database.GetConnectionString());
        await conn.ExecuteAsync(new CommandDefinition(sql, row, cancellationToken: cancellationToken));
    }
}
