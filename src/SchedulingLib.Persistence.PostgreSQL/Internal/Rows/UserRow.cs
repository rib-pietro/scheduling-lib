namespace SchedulingLib.Persistence.PostgreSQL.Internal.Rows;

internal sealed class UserRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
