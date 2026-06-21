using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Models;

namespace SchedulingLib.Services.Interfaces;

/// <summary>
/// Orchestrates booking, cancellation, and availability queries for service appointments.
/// </summary>
public interface IServiceAppointmentService
{
    /// <summary>
    /// Books a service appointment. Validates the requested slot against the staff member's
    /// weekly schedule and existing bookings before creating the appointment.
    /// </summary>
    Task<Result<ServiceAppointment>> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels the appointment with <paramref name="appointmentId"/>.
    /// Removes the event from the connected calendar if one is configured.
    /// </summary>
    Task<Result<bool>> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the available time slots for <paramref name="staffMemberId"/> on <paramref name="date"/>,
    /// derived from the staff member's weekly schedule minus already-booked slots.
    /// </summary>
    Task<Result<IReadOnlyList<TimeSlot>>> GetAvailableSlotsAsync(Guid staffMemberId, DateOnly date, CancellationToken cancellationToken = default);
}
