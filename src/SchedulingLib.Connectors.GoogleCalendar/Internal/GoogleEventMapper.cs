using Google.Apis.Calendar.v3.Data;
using SchedulingLib.Core.Models;

namespace SchedulingLib.Connectors.GoogleCalendar.Internal;

/// <summary>
/// Maps between library model types and Google Calendar API data types.
/// </summary>
internal static class GoogleEventMapper
{
    /// <summary>
    /// Converts a <see cref="CalendarEventRequest"/> into a Google <see cref="Event"/>.
    /// </summary>
    internal static Event ToGoogleEvent(CalendarEventRequest request)
    {
        var timeZone = request.TimeZoneId ?? "UTC";
        return new Event
        {
            Summary = request.Title,
            Description = request.Description,
            Start = new EventDateTime
            {
                DateTimeDateTimeOffset = request.Start,
                TimeZone = timeZone,
            },
            End = new EventDateTime
            {
                DateTimeDateTimeOffset = request.End,
                TimeZone = timeZone,
            },
        };
    }

    /// <summary>
    /// Converts a Google <see cref="Event"/> into an <see cref="ExternalCalendarEvent"/>.
    /// </summary>
    internal static ExternalCalendarEvent FromGoogleEvent(Event googleEvent)
    {
        var start = googleEvent.Start.DateTimeDateTimeOffset
            ?? DateTimeOffset.Parse(googleEvent.Start.Date!);
        var end = googleEvent.End.DateTimeDateTimeOffset
            ?? DateTimeOffset.Parse(googleEvent.End.Date!);

        return new ExternalCalendarEvent(
            googleEvent.Id,
            googleEvent.Summary ?? string.Empty,
            start,
            end);
    }
}
