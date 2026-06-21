namespace SchedulingLib.Core.Models;

/// <summary>
/// An event retrieved from an external calendar provider.
/// </summary>
public record ExternalCalendarEvent(
    string ExternalId,
    string Title,
    DateTimeOffset Start,
    DateTimeOffset End);
