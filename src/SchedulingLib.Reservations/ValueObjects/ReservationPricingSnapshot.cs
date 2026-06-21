namespace SchedulingLib.Reservations.ValueObjects;

/// <summary>
/// An immutable snapshot of pricing information captured at reservation creation time.
/// </summary>
public record ReservationPricingSnapshot(
    decimal NightlyRate,
    int NumberOfNights,
    decimal TotalAmount,
    string Currency);
