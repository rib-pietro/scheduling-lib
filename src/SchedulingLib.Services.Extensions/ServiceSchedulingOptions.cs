namespace SchedulingLib.Services.Extensions;

/// <summary>
/// Options for configuring the service-scheduling domain.
/// </summary>
public record ServiceSchedulingOptions
{
    /// <summary>Gets or inits the IANA time zone ID used when syncing appointments to a calendar.</summary>
    public string DefaultTimeZoneId { get; init; } = "UTC";
}
