using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Models;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Enums;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.Models;

namespace SchedulingLib.Services.Services;

/// <summary>
/// Default implementation of <see cref="IServiceAppointmentService"/>.
/// </summary>
public class ServiceAppointmentService : IServiceAppointmentService
{
    private readonly IStaffMemberRepository _staffMembers;
    private readonly IServiceAppointmentRepository _appointments;
    private readonly ICalendarConnector? _calendar;

    /// <summary>
    /// Initializes a new <see cref="ServiceAppointmentService"/>.
    /// </summary>
    /// <param name="staffMembers">Staff member persistence.</param>
    /// <param name="appointments">Appointment persistence.</param>
    /// <param name="calendar">Optional calendar connector; sync is skipped when null.</param>
    public ServiceAppointmentService(
        IStaffMemberRepository staffMembers,
        IServiceAppointmentRepository appointments,
        ICalendarConnector? calendar = null)
    {
        _staffMembers = staffMembers;
        _appointments = appointments;
        _calendar = calendar;
    }

    /// <inheritdoc />
    public async Task<Result<ServiceAppointment>> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffMembers.GetByIdAsync(request.StaffMemberId, cancellationToken);
        if (staffMember is null)
            return Result.Fail<ServiceAppointment>($"Staff member '{request.StaffMemberId}' not found.");

        if (!staffMember.Schedule.Contains(request.Date, request.RequestedSlot))
            return Result.Fail<ServiceAppointment>(
                $"The requested slot {request.RequestedSlot.Start}–{request.RequestedSlot.End} " +
                $"is outside the staff member's schedule on {request.Date.DayOfWeek}.");

        var existing = await _appointments.GetByStaffMemberAndDateAsync(request.StaffMemberId, request.Date, cancellationToken);
        var conflict = existing.FirstOrDefault(a =>
            a.Status != AppointmentStatus.Cancelled &&
            SlotsOverlap(a.TimeSlot, request.RequestedSlot));

        if (conflict is not null)
            return Result.Fail<ServiceAppointment>("The requested slot conflicts with an existing appointment.");

        var appointment = new ServiceAppointment(
            Guid.NewGuid(),
            request.StaffMemberId,
            request.ClientId,
            request.ServiceType,
            request.Date,
            request.RequestedSlot);

        appointment.Confirm();
        await _appointments.SaveAsync(appointment, cancellationToken);

        if (_calendar is not null)
        {
            var start = request.Date.ToDateTime(request.RequestedSlot.Start, DateTimeKind.Unspecified);
            var end = request.Date.ToDateTime(request.RequestedSlot.End, DateTimeKind.Unspecified);
            var calRequest = new CalendarEventRequest(
                appointment.Title,
                $"Service: {request.ServiceType.Name}",
                new DateTimeOffset(start),
                new DateTimeOffset(end),
                null);

            var calResult = await _calendar.CreateEventAsync(calRequest, cancellationToken);
            if (calResult.IsSuccess && calResult.Value is not null)
                appointment.AttachExternalId(calResult.Value);
        }

        return Result.Ok(appointment);
    }

    /// <inheritdoc />
    public async Task<Result<bool>> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointments.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<bool>($"Appointment '{appointmentId}' not found.");

        var cancelResult = appointment.Cancel();
        if (!cancelResult.IsSuccess)
            return Result.Fail<bool>(cancelResult.ErrorMessage!);

        await _appointments.SaveAsync(appointment, cancellationToken);

        if (_calendar is not null && appointment.ExternalCalendarEventId is not null)
            await _calendar.DeleteEventAsync(appointment.ExternalCalendarEventId, cancellationToken);

        return Result.Ok(true);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<TimeSlot>>> GetAvailableSlotsAsync(Guid staffMemberId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var staffMember = await _staffMembers.GetByIdAsync(staffMemberId, cancellationToken);
        if (staffMember is null)
            return Result.Fail<IReadOnlyList<TimeSlot>>($"Staff member '{staffMemberId}' not found.");

        var scheduledSlots = staffMember.Schedule.GetSlotsFor(date);
        if (scheduledSlots.Count == 0)
            return Result.Ok<IReadOnlyList<TimeSlot>>([]);

        var existing = await _appointments.GetByStaffMemberAndDateAsync(staffMemberId, date, cancellationToken);
        var bookedSlots = existing
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.TimeSlot)
            .ToList();

        var available = scheduledSlots
            .Where(slot => !bookedSlots.Any(booked => SlotsOverlap(booked, slot)))
            .ToList();

        return Result.Ok<IReadOnlyList<TimeSlot>>(available);
    }

    private static bool SlotsOverlap(TimeSlot a, TimeSlot b) =>
        a.Start < b.End && b.Start < a.End;
}
