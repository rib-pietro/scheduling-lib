using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Services.Enums;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Entities;

/// <summary>
/// A single-day, time-bounded appointment between a client and a <see cref="StaffMember"/>.
/// </summary>
public class ServiceAppointment : IScheduleEvent
{
    /// <summary>Gets the unique identifier.</summary>
    public Guid Id { get; }

    /// <summary>Gets the appointment title (e.g., "Haircut with Alice").</summary>
    public string Title { get; }

    /// <summary>Gets when this appointment was created.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>Gets the external calendar event ID after sync, or null if not synced.</summary>
    public string? ExternalCalendarEventId { get; private set; }

    /// <summary>Gets the ID of the staff member delivering the service.</summary>
    public Guid StaffMemberId { get; }

    /// <summary>Gets the ID of the client receiving the service.</summary>
    public Guid ClientId { get; }

    /// <summary>Gets the type of service being performed.</summary>
    public ServiceType ServiceType { get; }

    /// <summary>Gets the date on which the appointment takes place.</summary>
    public DateOnly Date { get; }

    /// <summary>Gets the time slot within the day.</summary>
    public TimeSlot TimeSlot { get; }

    /// <summary>Gets the current lifecycle status of the appointment.</summary>
    public AppointmentStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new <see cref="ServiceAppointment"/> in <see cref="AppointmentStatus.Pending"/> status.
    /// </summary>
    public ServiceAppointment(Guid id, Guid staffMemberId, Guid clientId, ServiceType serviceType, DateOnly date, TimeSlot timeSlot)
    {
        Id = id;
        StaffMemberId = staffMemberId;
        ClientId = clientId;
        ServiceType = serviceType;
        Date = date;
        TimeSlot = timeSlot;
        Status = AppointmentStatus.Pending;
        Title = $"{serviceType.Name}";
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Transitions the appointment to <see cref="AppointmentStatus.Confirmed"/>.
    /// Returns a failure result if the appointment is not in <see cref="AppointmentStatus.Pending"/> status.
    /// </summary>
    public Result Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            return Result.Fail($"Cannot confirm an appointment in '{Status}' status.");

        Status = AppointmentStatus.Confirmed;
        return Result.Ok();
    }

    /// <summary>
    /// Transitions the appointment to <see cref="AppointmentStatus.Cancelled"/>.
    /// Returns a failure result if the appointment is already cancelled.
    /// </summary>
    public Result Cancel()
    {
        if (Status == AppointmentStatus.Cancelled)
            return Result.Fail("The appointment is already cancelled.");

        Status = AppointmentStatus.Cancelled;
        return Result.Ok();
    }

    /// <summary>
    /// Records the external calendar event ID assigned by the calendar provider after sync.
    /// </summary>
    public void AttachExternalId(string externalId) => ExternalCalendarEventId = externalId;
}
