using SchedulingLib.Core.Models;
using SchedulingLib.Core.Results;

namespace SchedulingLib.Core.Abstractions;

/// <summary>
/// Abstraction for external calendar providers. Implement this to add a new connector (e.g., Outlook, iCal).
/// </summary>
public interface ICalendarConnector
{
    /// <summary>Gets the unique name identifying this calendar provider.</summary>
    string ProviderName { get; }

    /// <summary>
    /// Creates a new event in the external calendar and returns the provider-assigned event ID.
    /// </summary>
    Task<Result<string>> CreateEventAsync(CalendarEventRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing event identified by <paramref name="externalEventId"/>.
    /// </summary>
    Task<Result<bool>> UpdateEventAsync(string externalEventId, CalendarEventRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the event identified by <paramref name="externalEventId"/>.
    /// </summary>
    Task<Result<bool>> DeleteEventAsync(string externalEventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all events in the external calendar within the given time window.
    /// </summary>
    Task<Result<IReadOnlyList<ExternalCalendarEvent>>> GetEventsAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}
