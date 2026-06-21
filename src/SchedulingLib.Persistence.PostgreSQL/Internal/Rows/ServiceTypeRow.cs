namespace SchedulingLib.Persistence.PostgreSQL.Internal.Rows;

internal sealed class ServiceTypeRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public long DurationTicks { get; set; }
}
