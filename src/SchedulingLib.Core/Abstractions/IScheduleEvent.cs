namespace SchedulingLib.Core.Abstractions;

/// <summary>
/// Common contract shared by all schedule event types in the library.
/// </summary>
public interface IScheduleEvent
{
    /// <summary>Gets the unique identifier of the event.</summary>
    Guid Id { get; }

    /// <summary>Gets the human-readable title of the event.</summary>
    string Title { get; }

    /// <summary>Gets when the event was created.</summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the event identifier assigned by the external calendar provider after sync,
    /// or null if the event has not been synced yet.
    /// </summary>
    string? ExternalCalendarEventId { get; }
}
