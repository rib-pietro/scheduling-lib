using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Reservations.Enums;
using SchedulingLib.Reservations.ValueObjects;

namespace SchedulingLib.Reservations.Entities;

/// <summary>
/// A multi-day booking of a <see cref="ReservableResource"/> by a guest.
/// </summary>
public class Reservation : IScheduleEvent
{
    /// <summary>Gets the unique identifier.</summary>
    public Guid Id { get; }

    /// <summary>Gets the reservation title (e.g., the resource name).</summary>
    public string Title { get; }

    /// <summary>Gets when this reservation was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Gets the external calendar event ID after sync, or null if not synced.</summary>
    public string? ExternalCalendarEventId { get; private set; }

    /// <summary>Gets the ID of the resource being reserved.</summary>
    public Guid ResourceId { get; }

    /// <summary>Gets the ID of the guest making the reservation.</summary>
    public Guid GuestId { get; }

    /// <summary>Gets the date range of the stay (check-in to check-out).</summary>
    public DateRange DateRange { get; }

    /// <summary>Gets the current lifecycle status.</summary>
    public ReservationStatus Status { get; private set; }

    /// <summary>Gets the pricing snapshot captured at reservation creation, if provided.</summary>
    public ReservationPricingSnapshot? Pricing { get; }

    /// <summary>
    /// Initializes a new <see cref="Reservation"/> in <see cref="ReservationStatus.PendingConfirmation"/> status.
    /// </summary>
    public Reservation(Guid id, string title, Guid resourceId, Guid guestId, DateRange dateRange, ReservationPricingSnapshot? pricing)
    {
        Id = id;
        Title = title;
        ResourceId = resourceId;
        GuestId = guestId;
        DateRange = dateRange;
        Pricing = pricing;
        Status = ReservationStatus.PendingConfirmation;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Transitions to <see cref="ReservationStatus.Confirmed"/>.
    /// Returns a failure result if not in <see cref="ReservationStatus.PendingConfirmation"/>.
    /// </summary>
    public Result Confirm()
    {
        if (Status != ReservationStatus.PendingConfirmation)
            return Result.Fail($"Cannot confirm a reservation in '{Status}' status.");

        Status = ReservationStatus.Confirmed;
        return Result.Ok();
    }

    /// <summary>
    /// Transitions to <see cref="ReservationStatus.CheckedIn"/>.
    /// Returns a failure result if not in <see cref="ReservationStatus.Confirmed"/>.
    /// </summary>
    public Result CheckIn()
    {
        if (Status != ReservationStatus.Confirmed)
            return Result.Fail($"Cannot check in a reservation in '{Status}' status.");

        Status = ReservationStatus.CheckedIn;
        return Result.Ok();
    }

    /// <summary>
    /// Transitions to <see cref="ReservationStatus.CheckedOut"/>.
    /// Returns a failure result if not in <see cref="ReservationStatus.CheckedIn"/>.
    /// </summary>
    public Result CheckOut()
    {
        if (Status != ReservationStatus.CheckedIn)
            return Result.Fail($"Cannot check out a reservation in '{Status}' status.");

        Status = ReservationStatus.CheckedOut;
        return Result.Ok();
    }

    /// <summary>
    /// Transitions to <see cref="ReservationStatus.Cancelled"/>.
    /// Returns a failure result if already cancelled or checked out.
    /// </summary>
    public Result Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
            return Result.Fail("The reservation is already cancelled.");
        if (Status == ReservationStatus.CheckedOut)
            return Result.Fail("Cannot cancel a reservation that has already been checked out.");

        Status = ReservationStatus.Cancelled;
        return Result.Ok();
    }

    /// <summary>
    /// Records the external calendar event ID assigned by the calendar provider after sync.
    /// </summary>
    public void AttachExternalId(string externalId) => ExternalCalendarEventId = externalId;
}
