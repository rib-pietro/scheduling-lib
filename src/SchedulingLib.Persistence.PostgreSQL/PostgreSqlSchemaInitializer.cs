using Dapper;
using Npgsql;

namespace SchedulingLib.Persistence.PostgreSQL;

/// <summary>
/// Creates the scheduling schema in a PostgreSQL database.
/// Call <see cref="InitializeAsync"/> once at application startup or in test fixtures
/// before using the repositories.
/// </summary>
public static class PostgreSqlSchemaInitializer
{
    private const string Sql = """
        CREATE TABLE IF NOT EXISTS service_types (
            id             UUID PRIMARY KEY,
            name           TEXT NOT NULL,
            price          NUMERIC(18,4) NOT NULL,
            duration_ticks BIGINT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS staff_members (
            id                  UUID PRIMARY KEY,
            name                TEXT NOT NULL,
            email               TEXT NOT NULL,
            profile_picture_url TEXT,
            created_at          TIMESTAMPTZ NOT NULL,
            schedule            JSONB NOT NULL,
            offered_services    JSONB NOT NULL,
            gallery             JSONB NOT NULL DEFAULT '[]'
        );

        CREATE TABLE IF NOT EXISTS service_appointments (
            id                         UUID PRIMARY KEY,
            title                      TEXT NOT NULL,
            staff_member_id            UUID NOT NULL,
            client_id                  UUID NOT NULL,
            service_type_id            UUID NOT NULL REFERENCES service_types(id),
            date                       DATE NOT NULL,
            time_slot_start            TIME NOT NULL,
            time_slot_end              TIME NOT NULL,
            status                     TEXT NOT NULL,
            external_calendar_event_id TEXT,
            created_at                 TIMESTAMPTZ NOT NULL
        );

        CREATE INDEX IF NOT EXISTS idx_service_appointments_staff_date
            ON service_appointments (staff_member_id, date);
        """;

    /// <summary>
    /// Creates all scheduling tables and indexes if they do not already exist.
    /// Safe to call multiple times (idempotent).
    /// </summary>
    public static async Task InitializeAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.ExecuteAsync(new CommandDefinition(Sql, cancellationToken: cancellationToken));
    }
}
