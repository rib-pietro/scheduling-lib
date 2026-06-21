namespace SchedulingLib.Core.Primitives;

/// <summary>
/// A continuous range of calendar dates (inclusive on both ends).
/// </summary>
public record DateRange
{
    /// <summary>Gets the first day of the range (inclusive).</summary>
    public DateOnly Start { get; }

    /// <summary>Gets the last day of the range (inclusive).</summary>
    public DateOnly End { get; }

    /// <summary>
    /// Initializes a new <see cref="DateRange"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="end"/> is before <paramref name="start"/>.</exception>
    public DateRange(DateOnly start, DateOnly end)
    {
        if (end < start)
            throw new ArgumentException("End must not be before Start.", nameof(end));

        Start = start;
        End = end;
    }

    /// <summary>Gets the number of nights spanned by this range.</summary>
    public int Nights => End.DayNumber - Start.DayNumber;

    /// <summary>Returns true if this range overlaps with <paramref name="other"/>.</summary>
    public bool Overlaps(DateRange other) => Start <= other.End && End >= other.Start;

    /// <summary>Returns true if this range contains <paramref name="date"/>.</summary>
    public bool Contains(DateOnly date) => date >= Start && date <= End;
}
