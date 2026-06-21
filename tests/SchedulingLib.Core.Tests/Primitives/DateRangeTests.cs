using SchedulingLib.Core.Primitives;

namespace SchedulingLib.Core.Tests.Primitives;

public class DateRangeTests
{
    [Fact]
    public void Constructor_EndAfterStart_CreatesRange()
    {
        var range = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 5));
        Assert.Equal(new DateOnly(2025, 6, 1), range.Start);
        Assert.Equal(new DateOnly(2025, 6, 5), range.End);
    }

    [Fact]
    public void Constructor_SameDay_CreatesZeroNightRange()
    {
        var range = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 1));
        Assert.Equal(0, range.Nights);
    }

    [Fact]
    public void Constructor_EndBeforeStart_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new DateRange(new DateOnly(2025, 6, 5), new DateOnly(2025, 6, 1)));
    }

    [Fact]
    public void Nights_ReturnsCorrectCount()
    {
        var range = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 5));
        Assert.Equal(4, range.Nights);
    }

    [Fact]
    public void Overlaps_OverlappingRanges_ReturnsTrue()
    {
        var a = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 5));
        var b = new DateRange(new DateOnly(2025, 6, 3), new DateOnly(2025, 6, 8));
        Assert.True(a.Overlaps(b));
    }

    [Fact]
    public void Overlaps_NonOverlappingRanges_ReturnsFalse()
    {
        var a = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 5));
        var b = new DateRange(new DateOnly(2025, 6, 6), new DateOnly(2025, 6, 10));
        Assert.False(a.Overlaps(b));
    }

    [Fact]
    public void Contains_DateInsideRange_ReturnsTrue()
    {
        var range = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 10));
        Assert.True(range.Contains(new DateOnly(2025, 6, 5)));
    }

    [Fact]
    public void Contains_DateOutsideRange_ReturnsFalse()
    {
        var range = new DateRange(new DateOnly(2025, 6, 1), new DateOnly(2025, 6, 10));
        Assert.False(range.Contains(new DateOnly(2025, 6, 15)));
    }
}
