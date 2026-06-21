using SchedulingLib.Core.Primitives;

namespace SchedulingLib.Core.Tests.Primitives;

public class WeeklyScheduleTests
{
    private static WeeklySchedule BuildWeekdaySchedule()
    {
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var days = new[]
        {
            new DayOfWeekSchedule(DayOfWeek.Monday, [slot]),
            new DayOfWeekSchedule(DayOfWeek.Tuesday, [slot]),
            new DayOfWeekSchedule(DayOfWeek.Wednesday, [slot]),
        };
        return new WeeklySchedule(days);
    }

    [Fact]
    public void GetSlotsFor_ScheduledDay_ReturnsSlots()
    {
        var schedule = BuildWeekdaySchedule();
        var monday = new DateOnly(2025, 6, 2); // a Monday
        var slots = schedule.GetSlotsFor(monday);
        Assert.Single(slots);
    }

    [Fact]
    public void GetSlotsFor_UnscheduledDay_ReturnsEmpty()
    {
        var schedule = BuildWeekdaySchedule();
        var saturday = new DateOnly(2025, 6, 7);
        var slots = schedule.GetSlotsFor(saturday);
        Assert.Empty(slots);
    }

    [Fact]
    public void Contains_SlotWithinScheduledWindow_ReturnsTrue()
    {
        var schedule = BuildWeekdaySchedule();
        var monday = new DateOnly(2025, 6, 2);
        var inWindowSlot = new TimeSlot(new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.True(schedule.Contains(monday, inWindowSlot));
    }

    [Fact]
    public void Contains_SlotOutsideScheduledWindow_ReturnsFalse()
    {
        var schedule = BuildWeekdaySchedule();
        var monday = new DateOnly(2025, 6, 2);
        var outsideSlot = new TimeSlot(new TimeOnly(18, 0), new TimeOnly(19, 0));
        Assert.False(schedule.Contains(monday, outsideSlot));
    }

    [Fact]
    public void Contains_SlotOnUnscheduledDay_ReturnsFalse()
    {
        var schedule = BuildWeekdaySchedule();
        var saturday = new DateOnly(2025, 6, 7);
        var slot = new TimeSlot(new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.False(schedule.Contains(saturday, slot));
    }

    [Fact]
    public void ContainsDay_ScheduledDay_ReturnsTrue()
    {
        var schedule = BuildWeekdaySchedule();
        Assert.True(schedule.ContainsDay(new DateOnly(2025, 6, 2))); // Monday
    }

    [Fact]
    public void ContainsDay_UnscheduledDay_ReturnsFalse()
    {
        var schedule = BuildWeekdaySchedule();
        Assert.False(schedule.ContainsDay(new DateOnly(2025, 6, 7))); // Saturday
    }
}
