namespace SchedulingLib.Core.Models;

/// <summary>
/// Describes a calendar event to be created or updated via an <see cref="Abstractions.ICalendarConnector"/>.
/// </summary>
public record CalendarEventRequest(
    string Title,
    string? Description,
    DateTimeOffset Start,
    DateTimeOffset End,
    string? TimeZoneId);
