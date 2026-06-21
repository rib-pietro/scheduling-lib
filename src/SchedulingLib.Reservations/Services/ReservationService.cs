using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Models;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Reservations.Entities;
using SchedulingLib.Reservations.Enums;
using SchedulingLib.Reservations.Interfaces;
using SchedulingLib.Reservations.Models;

namespace SchedulingLib.Reservations.Services;

/// <summary>
/// Default implementation of <see cref="IReservationService"/>.
/// </summary>
public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservations;
    private readonly IResourceRepository _resources;
    private readonly ICalendarConnector? _calendar;

    /// <summary>
    /// Initializes a new <see cref="ReservationService"/>.
    /// </summary>
    /// <param name="reservations">Reservation persistence.</param>
    /// <param name="resources">Resource persistence.</param>
    /// <param name="calendar">Optional calendar connector; sync is skipped when null.</param>
    public ReservationService(
        IReservationRepository reservations,
        IResourceRepository resources,
        ICalendarConnector? calendar = null)
    {
        _reservations = reservations;
        _resources = resources;
        _calendar = calendar;
    }

    /// <inheritdoc />
    public async Task<Result<Reservation>> ReserveAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var resource = await _resources.GetByIdAsync(request.ResourceId, cancellationToken);
        if (resource is null)
            return Result.Fail<Reservation>($"Resource '{request.ResourceId}' not found.");

        if (!resource.AvailabilityWindow.ContainsDay(request.DateRange.Start))
            return Result.Fail<Reservation>(
                $"Check-in on {request.DateRange.Start.DayOfWeek} is not within the resource's availability window.");

        var overlapping = await _reservations.GetOverlappingAsync(request.ResourceId, request.DateRange, cancellationToken);
        if (overlapping.Count > 0)
            return Result.Fail<Reservation>("The requested dates overlap with an existing reservation.");

        var reservation = new Reservation(
            Guid.NewGuid(),
            request.ResourceName,
            request.ResourceId,
            request.GuestId,
            request.DateRange,
            request.Pricing);

        reservation.Confirm();
        await _reservations.SaveAsync(reservation, cancellationToken);

        if (_calendar is not null)
        {
            var start = request.DateRange.Start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
            var end = request.DateRange.End.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
            var calRequest = new CalendarEventRequest(
                reservation.Title,
                $"Reservation: {request.DateRange.Nights} night(s)",
                new DateTimeOffset(start),
                new DateTimeOffset(end),
                null);

            var calResult = await _calendar.CreateEventAsync(calRequest, cancellationToken);
            if (calResult.IsSuccess && calResult.Value is not null)
                reservation.AttachExternalId(calResult.Value);
        }

        return Result.Ok(reservation);
    }

    /// <inheritdoc />
    public async Task<Result<bool>> CancelAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await _reservations.GetByIdAsync(reservationId, cancellationToken);
        if (reservation is null)
            return Result.Fail<bool>($"Reservation '{reservationId}' not found.");

        var cancelResult = reservation.Cancel();
        if (!cancelResult.IsSuccess)
            return Result.Fail<bool>(cancelResult.ErrorMessage!);

        await _reservations.SaveAsync(reservation, cancellationToken);

        if (_calendar is not null && reservation.ExternalCalendarEventId is not null)
            await _calendar.DeleteEventAsync(reservation.ExternalCalendarEventId, cancellationToken);

        return Result.Ok(true);
    }

    /// <inheritdoc />
    public async Task<Result<bool>> IsAvailableAsync(Guid resourceId, DateRange range, CancellationToken cancellationToken = default)
    {
        var overlapping = await _reservations.GetOverlappingAsync(resourceId, range, cancellationToken);
        return Result.Ok(overlapping.Count == 0);
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<DateRange>>> GetUnavailableDatesAsync(Guid resourceId, int year, int month, CancellationToken cancellationToken = default)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        var monthRange = new DateRange(start, end);

        var overlapping = await _reservations.GetOverlappingAsync(resourceId, monthRange, cancellationToken);
        var unavailable = overlapping
            .Where(r => r.Status != ReservationStatus.Cancelled)
            .Select(r => r.DateRange)
            .ToList();

        return Result.Ok<IReadOnlyList<DateRange>>(unavailable);
    }
}
