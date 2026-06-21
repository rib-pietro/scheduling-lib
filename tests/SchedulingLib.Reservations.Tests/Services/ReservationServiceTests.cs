using Moq;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Models;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Reservations.Entities;
using SchedulingLib.Reservations.Interfaces;
using SchedulingLib.Reservations.Models;
using SchedulingLib.Reservations.Services;
using SchedulingLib.Reservations.ValueObjects;

namespace SchedulingLib.Reservations.Tests.Services;

public class ReservationServiceTests
{
    private static readonly DateRange TestRange = new(new DateOnly(2025, 7, 5), new DateOnly(2025, 7, 10)); // Saturday check-in

    private static ReservableResource CreateResource(WeeklySchedule schedule) =>
        new ReservableResource(Guid.NewGuid(), "Seaside Villa", "A nice villa", 4, schedule);

    private static WeeklySchedule WeekendSchedule()
    {
        var slot = new TimeSlot(new TimeOnly(15, 0), new TimeOnly(20, 0));
        return new WeeklySchedule(
        [
            new DayOfWeekSchedule(DayOfWeek.Saturday, [slot]),
            new DayOfWeekSchedule(DayOfWeek.Sunday, [slot]),
        ]);
    }

    private static CreateReservationRequest CreateRequest(Guid resourceId) =>
        new(resourceId, Guid.NewGuid(), "Seaside Villa", TestRange, null);

    [Fact]
    public async Task ReserveAsync_AvailableResourceAndValidCheckInDay_Succeeds()
    {
        var resource = CreateResource(WeekendSchedule());
        var resourceRepo = new Mock<IResourceRepository>();
        var reservationRepo = new Mock<IReservationRepository>();
        resourceRepo.Setup(r => r.GetByIdAsync(resource.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resource);
        reservationRepo.Setup(r => r.GetOverlappingAsync(resource.Id, TestRange, It.IsAny<CancellationToken>()))
                       .ReturnsAsync([]);

        var sut = new ReservationService(reservationRepo.Object, resourceRepo.Object);
        var result = await sut.ReserveAsync(CreateRequest(resource.Id));

        Assert.True(result.IsSuccess);
        reservationRepo.Verify(r => r.SaveAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReserveAsync_CheckInOnUnavailableDay_Fails()
    {
        // TestRange starts on Saturday; Monday-only schedule should reject it
        var mondayOnly = new WeeklySchedule(
        [
            new DayOfWeekSchedule(DayOfWeek.Monday, [new TimeSlot(new TimeOnly(15, 0), new TimeOnly(20, 0))])
        ]);
        var resource = CreateResource(mondayOnly);
        var resourceRepo = new Mock<IResourceRepository>();
        resourceRepo.Setup(r => r.GetByIdAsync(resource.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resource);

        var sut = new ReservationService(Mock.Of<IReservationRepository>(), resourceRepo.Object);
        var result = await sut.ReserveAsync(CreateRequest(resource.Id));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ReserveAsync_OverlappingReservation_Fails()
    {
        var resource = CreateResource(WeekendSchedule());
        var existing = new Reservation(Guid.NewGuid(), "Villa", resource.Id, Guid.NewGuid(), TestRange, null);
        existing.Confirm();

        var resourceRepo = new Mock<IResourceRepository>();
        var reservationRepo = new Mock<IReservationRepository>();
        resourceRepo.Setup(r => r.GetByIdAsync(resource.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resource);
        reservationRepo.Setup(r => r.GetOverlappingAsync(resource.Id, TestRange, It.IsAny<CancellationToken>()))
                       .ReturnsAsync([existing]);

        var sut = new ReservationService(reservationRepo.Object, resourceRepo.Object);
        var result = await sut.ReserveAsync(CreateRequest(resource.Id));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ReserveAsync_WithCalendarConnector_AttachesExternalId()
    {
        var resource = CreateResource(WeekendSchedule());
        var resourceRepo = new Mock<IResourceRepository>();
        var reservationRepo = new Mock<IReservationRepository>();
        var calendar = new Mock<ICalendarConnector>();
        resourceRepo.Setup(r => r.GetByIdAsync(resource.Id, It.IsAny<CancellationToken>())).ReturnsAsync(resource);
        reservationRepo.Setup(r => r.GetOverlappingAsync(resource.Id, TestRange, It.IsAny<CancellationToken>()))
                       .ReturnsAsync([]);
        calendar.Setup(c => c.CreateEventAsync(It.IsAny<CalendarEventRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok("ext-res-xyz"));

        var sut = new ReservationService(reservationRepo.Object, resourceRepo.Object, calendar.Object);
        var result = await sut.ReserveAsync(CreateRequest(resource.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal("ext-res-xyz", result.Value!.ExternalCalendarEventId);
    }

    [Fact]
    public async Task IsAvailableAsync_NoOverlap_ReturnsTrue()
    {
        var reservationRepo = new Mock<IReservationRepository>();
        reservationRepo.Setup(r => r.GetOverlappingAsync(It.IsAny<Guid>(), TestRange, It.IsAny<CancellationToken>()))
                       .ReturnsAsync([]);

        var sut = new ReservationService(reservationRepo.Object, Mock.Of<IResourceRepository>());
        var result = await sut.IsAvailableAsync(Guid.NewGuid(), TestRange);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task CancelAsync_ExistingReservation_Succeeds()
    {
        var reservation = new Reservation(Guid.NewGuid(), "Villa", Guid.NewGuid(), Guid.NewGuid(), TestRange, null);
        reservation.Confirm();

        var reservationRepo = new Mock<IReservationRepository>();
        reservationRepo.Setup(r => r.GetByIdAsync(reservation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(reservation);

        var sut = new ReservationService(reservationRepo.Object, Mock.Of<IResourceRepository>());
        var result = await sut.CancelAsync(reservation.Id);

        Assert.True(result.IsSuccess);
        reservationRepo.Verify(r => r.SaveAsync(reservation, It.IsAny<CancellationToken>()), Times.Once);
    }
}
