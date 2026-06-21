namespace SchedulingLib.Core.Primitives;

/// <summary>
/// A contiguous time window within a single day.
/// </summary>
public record TimeSlot
{
    /// <summary>Gets the start of the slot (inclusive).</summary>
    public TimeOnly Start { get; }

    /// <summary>Gets the end of the slot (exclusive).</summary>
    public TimeOnly End { get; }

    /// <summary>
    /// Initializes a new <see cref="TimeSlot"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when <paramref name="end"/> is not after <paramref name="start"/>.</exception>
    public TimeSlot(TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            throw new ArgumentException("End must be after Start.", nameof(end));

        Start = start;
        End = end;
    }

    /// <summary>Gets the duration of this slot.</summary>
    public TimeSpan Duration => End - Start;

    /// <summary>Returns true if this slot fully contains <paramref name="other"/>.</summary>
    public bool Contains(TimeSlot other) => other.Start >= Start && other.End <= End;
}
