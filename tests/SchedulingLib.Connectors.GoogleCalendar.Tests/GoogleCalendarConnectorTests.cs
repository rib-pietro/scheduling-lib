using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Moq;
using SchedulingLib.Connectors.GoogleCalendar;
using SchedulingLib.Core.Models;

namespace SchedulingLib.Connectors.GoogleCalendar.Tests;

public class GoogleCalendarConnectorTests
{
    [Fact]
    public void ProviderName_IsGoogleCalendar()
    {
        var connector = CreateConnector();
        Assert.Equal("GoogleCalendar", connector.ProviderName);
    }

    [Fact]
    public async Task CreateEventAsync_ServiceThrows_ReturnsFailResult()
    {
        // The internal constructor accepts a real CalendarService; we use a fake base URL
        // so that any HTTP call fails, validating the exception-to-Result conversion.
        var service = new CalendarService(new BaseClientService.Initializer
        {
            ApiKey = "fake",
            BaseUri = "http://localhost:1/",
        });
        var connector = new GoogleCalendarConnector("primary", service, false);
        var request = new CalendarEventRequest("Test", null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1), null);

        var result = await connector.CreateEventAsync(request);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task DeleteEventAsync_ServiceThrows_ReturnsFailResult()
    {
        var service = new CalendarService(new BaseClientService.Initializer
        {
            ApiKey = "fake",
            BaseUri = "http://localhost:1/",
        });
        var connector = new GoogleCalendarConnector("primary", service, false);

        var result = await connector.DeleteEventAsync("event-id");

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetEventsAsync_ServiceThrows_ReturnsFailResult()
    {
        var service = new CalendarService(new BaseClientService.Initializer
        {
            ApiKey = "fake",
            BaseUri = "http://localhost:1/",
        });
        var connector = new GoogleCalendarConnector("primary", service, false);

        var result = await connector.GetEventsAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(7));

        Assert.False(result.IsSuccess);
    }

    private static GoogleCalendarConnector CreateConnector()
    {
        var service = new CalendarService(new BaseClientService.Initializer { ApiKey = "fake" });
        return new GoogleCalendarConnector("primary", service, false);
    }
}
