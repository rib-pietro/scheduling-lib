using SchedulingLib.Core.Primitives;
using SchedulingLib.Reservations.ValueObjects;

namespace SchedulingLib.Reservations.Models;

/// <summary>
/// Input required to create a reservation.
/// </summary>
public record CreateReservationRequest(
    Guid ResourceId,
    Guid GuestId,
    string ResourceName,
    DateRange DateRange,
    ReservationPricingSnapshot? Pricing);
