using SchedulingLib.Core.Primitives;

namespace SchedulingLib.Reservations.ValueObjects;

/// <summary>
/// A bookable resource (e.g., a rental property or accommodation unit).
/// The <see cref="AvailabilityWindow"/> defines which days of the week and time windows are valid for check-in.
/// </summary>
public record ReservableResource(
    Guid Id,
    string Name,
    string Description,
    int MaxOccupancy,
    WeeklySchedule AvailabilityWindow);
