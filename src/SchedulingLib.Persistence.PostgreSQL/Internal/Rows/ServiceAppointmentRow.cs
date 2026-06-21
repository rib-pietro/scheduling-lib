namespace SchedulingLib.Persistence.PostgreSQL.Internal.Rows;

internal sealed class ServiceAppointmentRow
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid StaffMemberId { get; set; }
    public Guid ClientId { get; set; }
    public Guid ServiceTypeId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly TimeSlotStart { get; set; }
    public TimeOnly TimeSlotEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ExternalCalendarEventId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
