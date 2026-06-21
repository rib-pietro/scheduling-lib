namespace SchedulingLib.Persistence.PostgreSQL.Internal.Rows;

internal sealed class StaffMemberRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string ScheduleJson { get; set; } = string.Empty;
    public string OfferedServicesJson { get; set; } = string.Empty;
    public string GalleryJson { get; set; } = "[]";
}
