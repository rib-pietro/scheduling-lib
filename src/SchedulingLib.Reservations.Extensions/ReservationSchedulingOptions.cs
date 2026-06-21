namespace SchedulingLib.Reservations.Extensions;

/// <summary>
/// Options for configuring the reservation-scheduling domain.
/// </summary>
public record ReservationSchedulingOptions
{
    /// <summary>Gets or inits the default currency code (ISO 4217) for pricing snapshots.</summary>
    public string DefaultCurrency { get; init; } = "USD";

    /// <summary>Gets or inits the standard check-in time of day.</summary>
    public TimeOnly CheckInTime { get; init; } = new(15, 0);

    /// <summary>Gets or inits the standard check-out time of day.</summary>
    public TimeOnly CheckOutTime { get; init; } = new(11, 0);
}
