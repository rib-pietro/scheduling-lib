namespace SchedulingLib.Services.ValueObjects;

/// <summary>
/// A photo in a <see cref="Entities.StaffMember"/>'s gallery, optionally tagged with the service types depicted.
/// </summary>
public record GalleryPhoto(string Url, IReadOnlyList<Guid> ServiceTypeIds, DateTimeOffset CreatedAt);
