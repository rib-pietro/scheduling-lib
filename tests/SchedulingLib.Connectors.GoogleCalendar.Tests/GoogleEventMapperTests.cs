using Google.Apis.Calendar.v3.Data;
using SchedulingLib.Connectors.GoogleCalendar.Internal;
using SchedulingLib.Core.Models;

namespace SchedulingLib.Connectors.GoogleCalendar.Tests;

public class GoogleEventMapperTests
{
    [Fact]
    public void ToGoogleEvent_MapsFieldsCorrectly()
    {
        var start = new DateTimeOffset(2025, 6, 10, 9, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2025, 6, 10, 10, 0, 0, TimeSpan.Zero);
        var request = new CalendarEventRequest("Haircut", "Service description", start, end, "America/New_York");

        var googleEvent = GoogleEventMapper.ToGoogleEvent(request);

        Assert.Equal("Haircut", googleEvent.Summary);
        Assert.Equal("Service description", googleEvent.Description);
        Assert.Equal(start, googleEvent.Start.DateTimeDateTimeOffset);
        Assert.Equal(end, googleEvent.End.DateTimeDateTimeOffset);
        Assert.Equal("America/New_York", googleEvent.Start.TimeZone);
        Assert.Equal("America/New_York", googleEvent.End.TimeZone);
    }

    [Fact]
    public void ToGoogleEvent_NullTimeZone_DefaultsToUtc()
    {
        var request = new CalendarEventRequest("Test", null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), null);

        var googleEvent = GoogleEventMapper.ToGoogleEvent(request);

        Assert.Equal("UTC", googleEvent.Start.TimeZone);
    }

    [Fact]
    public void FromGoogleEvent_MapsFieldsCorrectly()
    {
        var start = new DateTimeOffset(2025, 6, 10, 9, 0, 0, TimeSpan.Zero);
        var end = new DateTimeOffset(2025, 6, 10, 10, 0, 0, TimeSpan.Zero);
        var googleEvent = new Event
        {
            Id = "google-id-123",
            Summary = "Haircut",
            Start = new EventDateTime { DateTimeDateTimeOffset = start },
            End = new EventDateTime { DateTimeDateTimeOffset = end },
        };

        var externalEvent = GoogleEventMapper.FromGoogleEvent(googleEvent);

        Assert.Equal("google-id-123", externalEvent.ExternalId);
        Assert.Equal("Haircut", externalEvent.Title);
        Assert.Equal(start, externalEvent.Start);
        Assert.Equal(end, externalEvent.End);
    }
}
