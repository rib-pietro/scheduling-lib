using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using SchedulingLib.Connectors.GoogleCalendar.Internal;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Models;
using SchedulingLib.Core.Results;

namespace SchedulingLib.Connectors.GoogleCalendar;

/// <summary>
/// <see cref="ICalendarConnector"/> implementation backed by the Google Calendar API v3.
/// All Google API exceptions are caught and returned as <see cref="Result"/> failures — never rethrown.
/// Use <see cref="CreateAsync"/> to construct an instance.
/// </summary>
public class GoogleCalendarConnector : ICalendarConnector
{
    private readonly string _calendarId;
    private readonly CalendarService _service;

    /// <inheritdoc />
    public string ProviderName => "GoogleCalendar";

    private GoogleCalendarConnector(string calendarId, CalendarService service)
    {
        _calendarId = calendarId;
        _service = service;
    }

    /// <summary>Internal constructor for testing with a pre-built service.</summary>
    internal GoogleCalendarConnector(string calendarId, CalendarService service, bool _)
        : this(calendarId, service) { }

    /// <summary>
    /// Creates a new <see cref="GoogleCalendarConnector"/> using service-account credentials.
    /// </summary>
    public static async Task<GoogleCalendarConnector> CreateAsync(
        GoogleCalendarConnectorOptions options,
        CancellationToken cancellationToken = default)
    {
        var credentialFactory = new CredentialFactory();
        var credential = await credentialFactory.CreateServiceAccountCredentialFromJsonAsync(
            options.CredentialsJson, cancellationToken);

        var scoped = ((ServiceAccountCredential)credential).CreateWithScopes(CalendarService.Scope.Calendar);

        var service = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = scoped,
            ApplicationName = options.ApplicationName,
        });

        return new GoogleCalendarConnector(options.CalendarId, service);
    }

    /// <inheritdoc />
    public async Task<Result<string>> CreateEventAsync(CalendarEventRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var googleEvent = GoogleEventMapper.ToGoogleEvent(request);
            var inserted = await _service.Events
                .Insert(googleEvent, _calendarId)
                .ExecuteAsync(cancellationToken);

            return Result.Ok(inserted.Id);
        }
        catch (Exception ex)
        {
            return Result.Fail<string>($"Google Calendar create failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> UpdateEventAsync(string externalEventId, CalendarEventRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var googleEvent = GoogleEventMapper.ToGoogleEvent(request);
            await _service.Events
                .Update(googleEvent, _calendarId, externalEventId)
                .ExecuteAsync(cancellationToken);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Google Calendar update failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteEventAsync(string externalEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _service.Events
                .Delete(_calendarId, externalEventId)
                .ExecuteAsync(cancellationToken);

            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail<bool>($"Google Calendar delete failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ExternalCalendarEvent>>> GetEventsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        try
        {
            var listRequest = _service.Events.List(_calendarId);
            listRequest.TimeMinDateTimeOffset = from;
            listRequest.TimeMaxDateTimeOffset = to;
            listRequest.SingleEvents = true;
            listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var result = await listRequest.ExecuteAsync(cancellationToken);
            var events = result.Items?
                .Select(GoogleEventMapper.FromGoogleEvent)
                .ToList() ?? [];

            return Result.Ok<IReadOnlyList<ExternalCalendarEvent>>(events);
        }
        catch (Exception ex)
        {
            return Result.Fail<IReadOnlyList<ExternalCalendarEvent>>($"Google Calendar list failed: {ex.Message}");
        }
    }
}
