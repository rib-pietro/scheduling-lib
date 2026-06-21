using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SchedulingLib.Persistence.PostgreSQL.Internal;
using SchedulingLib.Persistence.PostgreSQL.Internal.Dapper;
using SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;
using SchedulingLib.Persistence.PostgreSQL.Internal.Rows;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Interfaces;

namespace SchedulingLib.Persistence.PostgreSQL.Repositories;

/// <summary>
/// PostgreSQL-backed implementation of <see cref="IServiceAppointmentRepository"/>.
/// Queries run through EF Core; upserts use Dapper for explicit SQL control.
/// Service type data is loaded via a secondary EF Core query (FK join pattern).
/// </summary>
internal sealed class PostgreSqlServiceAppointmentRepository(SchedulingDbContext context) : IServiceAppointmentRepository
{
    // Ensure Dapper can serialize DateOnly/TimeOnly regardless of how DI is configured.
    static PostgreSqlServiceAppointmentRepository()
    {
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
    }

    /// <inheritdoc />
    public async Task<ServiceAppointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await context.ServiceAppointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (row is null) return null;

        var serviceTypeRow = await context.ServiceTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == row.ServiceTypeId, cancellationToken);

        return serviceTypeRow is null ? null : ServiceAppointmentMapper.ToDomain(row, serviceTypeRow);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceAppointment>> GetByStaffMemberAndDateAsync(
        Guid staffMemberId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var rows = await context.ServiceAppointments
            .AsNoTracking()
            .Where(a => a.StaffMemberId == staffMemberId && a.Date == date)
            .ToListAsync(cancellationToken);

        if (rows.Count == 0) return [];

        var serviceTypeIds = rows.Select(r => r.ServiceTypeId).Distinct().ToList();
        var serviceTypes = await context.ServiceTypes
            .AsNoTracking()
            .Where(s => serviceTypeIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        return rows
            .Where(r => serviceTypes.ContainsKey(r.ServiceTypeId))
            .Select(r => ServiceAppointmentMapper.ToDomain(r, serviceTypes[r.ServiceTypeId]))
            .ToList();
    }

    /// <inheritdoc />
    public async Task SaveAsync(ServiceAppointment appointment, CancellationToken cancellationToken = default)
    {
        var row = ServiceAppointmentMapper.ToRow(appointment);

        const string sql = """
            INSERT INTO service_appointments (
                id, title, staff_member_id, client_id, service_type_id,
                date, time_slot_start, time_slot_end,
                status, external_calendar_event_id, created_at)
            VALUES (
                @Id, @Title, @StaffMemberId, @ClientId, @ServiceTypeId,
                @Date::date, @TimeSlotStart::time, @TimeSlotEnd::time,
                @Status, @ExternalCalendarEventId, @CreatedAt)
            ON CONFLICT (id) DO UPDATE SET
                status                     = EXCLUDED.status,
                external_calendar_event_id = EXCLUDED.external_calendar_event_id
            """;

        await using var conn = new NpgsqlConnection(context.Database.GetConnectionString());
        await conn.ExecuteAsync(new CommandDefinition(sql, row, cancellationToken: cancellationToken));
    }
}
