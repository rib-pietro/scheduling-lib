namespace SchedulingLib.Connectors.GoogleCalendar;

/// <summary>
/// Configuration options for the Google Calendar connector.
/// </summary>
public record GoogleCalendarConnectorOptions
{
    /// <summary>Gets or inits the Google Calendar ID (e.g., "primary" or a full calendar ID).</summary>
    public string CalendarId { get; init; } = "primary";

    /// <summary>Gets or inits the service-account credentials JSON.</summary>
    public string CredentialsJson { get; init; } = string.Empty;

    /// <summary>Gets or inits the application name registered with the Google API Console.</summary>
    public string ApplicationName { get; init; } = string.Empty;
}
