namespace SchedulingLib.Reservations.Enums;

/// <summary>
/// Lifecycle states of a <see cref="Entities.Reservation"/>.
/// </summary>
public enum ReservationStatus
{
    /// <summary>The reservation has been requested but not yet confirmed.</summary>
    PendingConfirmation,

    /// <summary>The reservation has been confirmed.</summary>
    Confirmed,

    /// <summary>The guest has checked in.</summary>
    CheckedIn,

    /// <summary>The guest has checked out.</summary>
    CheckedOut,

    /// <summary>The reservation has been cancelled.</summary>
    Cancelled,
}
