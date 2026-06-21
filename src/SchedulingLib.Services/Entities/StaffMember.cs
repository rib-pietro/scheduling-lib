using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Entities;

/// <summary>
/// A person who provides services (e.g., a hairdresser, therapist, or consultant).
/// </summary>
public class StaffMember : IScheduleEvent
{
    private readonly List<ServiceType> _offeredServices = [];
    private readonly List<GalleryPhoto> _gallery = [];

    /// <summary>Gets the unique identifier.</summary>
    public Guid Id { get; }

    /// <summary>Gets the staff member's display name (used as the event title for calendar sync).</summary>
    public string Title { get; private set; }

    /// <summary>Gets when this record was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Always null — staff members are not calendar events themselves.</summary>
    public string? ExternalCalendarEventId => null;

    /// <summary>Gets the staff member's full name.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the staff member's contact email.</summary>
    public string Email { get; private set; }

    /// <summary>Gets the weekly schedule that defines when this staff member is available.</summary>
    public WeeklySchedule Schedule { get; private set; }

    /// <summary>Gets the optional URL of the staff member's profile picture.</summary>
    public string? ProfilePictureUrl { get; private set; }

    /// <summary>Gets the services this staff member offers.</summary>
    public IReadOnlyList<ServiceType> OfferedServices => _offeredServices;

    /// <summary>Gets the gallery of photos associated with this staff member.</summary>
    public IReadOnlyList<GalleryPhoto> Gallery => _gallery;

    /// <summary>
    /// Initializes a new <see cref="StaffMember"/>.
    /// </summary>
    public StaffMember(Guid id, string name, string email, WeeklySchedule schedule)
    {
        Id = id;
        Name = name;
        Title = name;
        Email = email;
        Schedule = schedule;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Adds <paramref name="serviceType"/> to the offered services.
    /// Returns a failure result if the service type ID is already present.
    /// </summary>
    public Result AddService(ServiceType serviceType)
    {
        if (_offeredServices.Any(s => s.Id == serviceType.Id))
            return Result.Fail($"Service type '{serviceType.Id}' is already offered.");

        _offeredServices.Add(serviceType);
        return Result.Ok();
    }

    /// <summary>
    /// Removes the service type with <paramref name="serviceTypeId"/> from the offered services.
    /// Returns a failure result if the service type is not found.
    /// </summary>
    public Result RemoveService(Guid serviceTypeId)
    {
        var service = _offeredServices.FirstOrDefault(s => s.Id == serviceTypeId);
        if (service is null)
            return Result.Fail($"Service type '{serviceTypeId}' is not offered.");

        _offeredServices.Remove(service);
        return Result.Ok();
    }

    /// <summary>
    /// Replaces the staff member's weekly schedule with <paramref name="schedule"/>.
    /// </summary>
    public void UpdateSchedule(WeeklySchedule schedule) => Schedule = schedule;

    /// <summary>
    /// Sets the profile picture URL, or clears it when <paramref name="url"/> is null.
    /// </summary>
    public void UpdateProfilePicture(string? url) => ProfilePictureUrl = url;

    /// <summary>
    /// Appends <paramref name="photo"/> to the gallery.
    /// </summary>
    public void AddGalleryPhoto(GalleryPhoto photo) => _gallery.Add(photo);

    /// <summary>
    /// Removes the gallery photo with the given <paramref name="url"/>.
    /// Returns a failure result when no matching photo is found.
    /// </summary>
    public Result RemoveGalleryPhoto(string url)
    {
        var removed = _gallery.RemoveAll(p => p.Url == url);
        return removed > 0
            ? Result.Ok()
            : Result.Fail($"No gallery photo with URL '{url}' was found.");
    }
}
