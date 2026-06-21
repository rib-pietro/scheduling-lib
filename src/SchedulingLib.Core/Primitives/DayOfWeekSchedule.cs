namespace SchedulingLib.Core.Primitives;

/// <summary>
/// The available time windows for a specific day of the week.
/// </summary>
public record DayOfWeekSchedule(DayOfWeek Day, IReadOnlyList<TimeSlot> TimeSlots);
