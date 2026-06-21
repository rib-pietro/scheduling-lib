namespace SchedulingLib.Core.Primitives;

/// <summary>
/// A recurring weekly availability window, used as a guard for booking and reservation validation.
/// </summary>
public record WeeklySchedule(IReadOnlyList<DayOfWeekSchedule> Days)
{
    /// <summary>
    /// Returns the available time slots for <paramref name="date"/> based on its day of week,
    /// or an empty list if the day is not scheduled.
    /// </summary>
    public IReadOnlyList<TimeSlot> GetSlotsFor(DateOnly date)
    {
        var schedule = Days.FirstOrDefault(d => d.Day == date.DayOfWeek);
        return schedule?.TimeSlots ?? [];
    }

    /// <summary>
    /// Returns true if <paramref name="slot"/> falls within an available window on <paramref name="date"/>.
    /// </summary>
    public bool Contains(DateOnly date, TimeSlot slot)
    {
        var slots = GetSlotsFor(date);
        return slots.Any(s => s.Contains(slot));
    }

    /// <summary>
    /// Returns true if <paramref name="date"/>'s day of week is present in this schedule (regardless of time).
    /// </summary>
    public bool ContainsDay(DateOnly date) =>
        Days.Any(d => d.Day == date.DayOfWeek);
}
