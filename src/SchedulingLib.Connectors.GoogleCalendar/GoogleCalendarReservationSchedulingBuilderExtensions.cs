using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;

namespace SchedulingLib.Connectors.GoogleCalendar;

/// <summary>
/// Extension methods for adding the Google Calendar connector to the reservation-scheduling domain.
/// </summary>
public static class GoogleCalendarReservationSchedulingBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="GoogleCalendarConnector"/> as the <see cref="ICalendarConnector"/>
    /// for the reservation-scheduling domain.
    /// </summary>
    public static IReservationSchedulingBuilder AddGoogleCalendarConnector(
        this IReservationSchedulingBuilder builder,
        Action<GoogleCalendarConnectorOptions> configure)
    {
        var options = new GoogleCalendarConnectorOptions();
        configure(options);
        builder.Services.AddSingleton<ICalendarConnector>(
            _ => GoogleCalendarConnector.CreateAsync(options).GetAwaiter().GetResult());
        return builder;
    }
}
