using SchedulingLib.Core.Primitives;

namespace SchedulingLib.Core.Tests.Primitives;

public class TimeSlotTests
{
    [Fact]
    public void Constructor_EndAfterStart_CreatesSlot()
    {
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(10, 0));
        Assert.Equal(new TimeOnly(9, 0), slot.Start);
        Assert.Equal(new TimeOnly(10, 0), slot.End);
    }

    [Fact]
    public void Constructor_EndEqualsStart_Throws()
    {
        var time = new TimeOnly(9, 0);
        Assert.Throws<ArgumentException>(() => new TimeSlot(time, time));
    }

    [Fact]
    public void Constructor_EndBeforeStart_Throws()
    {
        Assert.Throws<ArgumentException>(() => new TimeSlot(new TimeOnly(10, 0), new TimeOnly(9, 0)));
    }

    [Fact]
    public void Duration_ReturnsCorrectSpan()
    {
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(10, 30));
        Assert.Equal(TimeSpan.FromMinutes(90), slot.Duration);
    }

    [Fact]
    public void Contains_SlotWithinBounds_ReturnsTrue()
    {
        var outer = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(11, 0));
        var inner = new TimeSlot(new TimeOnly(9, 30), new TimeOnly(10, 30));
        Assert.True(outer.Contains(inner));
    }

    [Fact]
    public void Contains_SlotExceedsBounds_ReturnsFalse()
    {
        var outer = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(10, 0));
        var exceeding = new TimeSlot(new TimeOnly(9, 30), new TimeOnly(10, 30));
        Assert.False(outer.Contains(exceeding));
    }

    [Fact]
    public void RecordEquality_SameTimes_AreEqual()
    {
        var a = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(10, 0));
        var b = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(10, 0));
        Assert.Equal(a, b);
    }
}
