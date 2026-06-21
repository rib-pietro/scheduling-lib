using System.Reflection;
using System.Text.Json;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Persistence.PostgreSQL.Internal.Json;
using SchedulingLib.Persistence.PostgreSQL.Internal.Rows;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;

internal static class StaffMemberMapper
{
    // The auto-property backing field for get-only CreatedAt, which is set once in the domain constructor.
    private static readonly FieldInfo CreatedAtField =
        typeof(StaffMember).GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly FieldInfo GalleryField =
        typeof(StaffMember).GetField("_gallery", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly PropertyInfo ProfilePictureProperty =
        typeof(StaffMember).GetProperty(nameof(StaffMember.ProfilePictureUrl))!;

    internal static StaffMemberRow ToRow(StaffMember staff) => new()
    {
        Id = staff.Id,
        Name = staff.Name,
        Email = staff.Email,
        ProfilePictureUrl = staff.ProfilePictureUrl,
        CreatedAt = staff.CreatedAt,
        ScheduleJson = JsonSerializer.Serialize(staff.Schedule, SchedulingJsonOptions.Default),
        OfferedServicesJson = JsonSerializer.Serialize(staff.OfferedServices, SchedulingJsonOptions.Default),
        GalleryJson = JsonSerializer.Serialize(staff.Gallery, SchedulingJsonOptions.Default),
    };

    internal static StaffMember ToDomain(StaffMemberRow row)
    {
        var schedule = JsonSerializer.Deserialize<WeeklySchedule>(row.ScheduleJson, SchedulingJsonOptions.Default)!;
        var staff = new StaffMember(row.Id, row.Name, row.Email, schedule);

        // The domain constructor always sets CreatedAt = DateTimeOffset.UtcNow; restore the persisted value.
        CreatedAtField.SetValue(staff, row.CreatedAt);
        ProfilePictureProperty.SetValue(staff, row.ProfilePictureUrl);

        var services = JsonSerializer.Deserialize<List<ServiceType>>(row.OfferedServicesJson, SchedulingJsonOptions.Default) ?? [];
        foreach (var service in services)
            staff.AddService(service);

        var gallery = JsonSerializer.Deserialize<List<GalleryPhoto>>(row.GalleryJson, SchedulingJsonOptions.Default) ?? [];
        var galleryList = (List<GalleryPhoto>)GalleryField.GetValue(staff)!;
        galleryList.AddRange(gallery);

        return staff;
    }
}
