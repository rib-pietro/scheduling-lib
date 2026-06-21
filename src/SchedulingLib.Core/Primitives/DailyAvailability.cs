namespace SchedulingLib.Core.Primitives;

/// <summary>
/// The available time slots for a specific calendar date.
/// </summary>
public record DailyAvailability(DateOnly Date, IReadOnlyList<TimeSlot> Slots);
