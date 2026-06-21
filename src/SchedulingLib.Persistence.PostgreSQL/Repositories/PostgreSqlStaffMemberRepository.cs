using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Interfaces;

namespace SchedulingLib.Persistence.PostgreSQL.Repositories;

/// <summary>
/// PostgreSQL-backed implementation of <see cref="IStaffMemberRepository"/>.
/// Queries run through EF Core; upserts use Dapper for explicit SQL control.
/// </summary>
internal sealed class PostgreSqlStaffMemberRepository(SchedulingDbContext context) : IStaffMemberRepository
{
    /// <inheritdoc />
    public async Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await context.StaffMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return row is null ? null : StaffMemberMapper.ToDomain(row);
    }

    /// <inheritdoc />
    public async Task SaveAsync(StaffMember staffMember, CancellationToken cancellationToken = default)
    {
        var row = StaffMemberMapper.ToRow(staffMember);

        const string sql = """
            INSERT INTO staff_members (id, name, email, profile_picture_url, created_at, schedule, offered_services, gallery)
            VALUES (@Id, @Name, @Email, @ProfilePictureUrl, @CreatedAt, @ScheduleJson::jsonb, @OfferedServicesJson::jsonb, @GalleryJson::jsonb)
            ON CONFLICT (id) DO UPDATE SET
                name                = EXCLUDED.name,
                email               = EXCLUDED.email,
                profile_picture_url = EXCLUDED.profile_picture_url,
                schedule            = EXCLUDED.schedule,
                offered_services    = EXCLUDED.offered_services,
                gallery             = EXCLUDED.gallery
            """;

        await using var conn = new NpgsqlConnection(context.Database.GetConnectionString());
        await conn.ExecuteAsync(new CommandDefinition(sql, row, cancellationToken: cancellationToken));
    }
}
