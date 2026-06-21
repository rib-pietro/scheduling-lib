using SchedulingLib.Core.Primitives;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Models;

/// <summary>
/// Input required to book a service appointment.
/// </summary>
public record BookAppointmentRequest(
    Guid StaffMemberId,
    Guid ClientId,
    ServiceType ServiceType,
    DateOnly Date,
    TimeSlot RequestedSlot);
