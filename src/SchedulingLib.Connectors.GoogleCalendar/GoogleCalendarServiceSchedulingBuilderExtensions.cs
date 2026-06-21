using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Abstractions;

namespace SchedulingLib.Connectors.GoogleCalendar;

/// <summary>
/// Extension methods for adding the Google Calendar connector to the service-scheduling domain.
/// </summary>
public static class GoogleCalendarServiceSchedulingBuilderExtensions
{
    /// <summary>
    /// Registers <see cref="GoogleCalendarConnector"/> as the <see cref="ICalendarConnector"/>
    /// for the service-scheduling domain.
    /// </summary>
    public static IServiceSchedulingBuilder AddGoogleCalendarConnector(
        this IServiceSchedulingBuilder builder,
        Action<GoogleCalendarConnectorOptions> configure)
    {
        var options = new GoogleCalendarConnectorOptions();
        configure(options);
        builder.Services.AddSingleton<ICalendarConnector>(
            _ => GoogleCalendarConnector.CreateAsync(options).GetAwaiter().GetResult());
        return builder;
    }
}
