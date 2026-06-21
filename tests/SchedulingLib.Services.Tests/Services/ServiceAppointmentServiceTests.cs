using Moq;
using SchedulingLib.Core.Abstractions;
using SchedulingLib.Core.Models;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Core.Results;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.Models;
using SchedulingLib.Services.Services;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Tests.Services;

public class ServiceAppointmentServiceTests
{
    private static readonly DateOnly TestDate = new(2025, 6, 2); // Monday
    private static readonly TimeSlot WorkingHours = new(new TimeOnly(9, 0), new TimeOnly(17, 0));
    private static readonly TimeSlot BookedSlot = new(new TimeOnly(10, 0), new TimeOnly(11, 0));
    private static readonly ServiceType Haircut = new(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromHours(1));

    private static StaffMember CreateStaffMember()
    {
        var schedule = new WeeklySchedule(
        [
            new DayOfWeekSchedule(DayOfWeek.Monday, [WorkingHours])
        ]);
        return new StaffMember(Guid.NewGuid(), "Alice", "alice@example.com", schedule);
    }

    private static BookAppointmentRequest CreateRequest(StaffMember member, TimeSlot slot) =>
        new(member.Id, Guid.NewGuid(), Haircut, TestDate, slot);

    [Fact]
    public async Task BookAsync_ValidSlot_CreatesConfirmedAppointment()
    {
        var member = CreateStaffMember();
        var staffRepo = new Mock<IStaffMemberRepository>();
        var aptRepo = new Mock<IServiceAppointmentRepository>();
        staffRepo.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        aptRepo.Setup(r => r.GetByStaffMemberAndDateAsync(member.Id, TestDate, It.IsAny<CancellationToken>()))
               .ReturnsAsync([]);

        var sut = new ServiceAppointmentService(staffRepo.Object, aptRepo.Object);
        var result = await sut.BookAsync(CreateRequest(member, BookedSlot));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        aptRepo.Verify(r => r.SaveAsync(It.IsAny<ServiceAppointment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BookAsync_SlotOutsideSchedule_Fails()
    {
        var member = CreateStaffMember();
        var staffRepo = new Mock<IStaffMemberRepository>();
        staffRepo.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>())).ReturnsAsync(member);

        var sut = new ServiceAppointmentService(staffRepo.Object, Mock.Of<IServiceAppointmentRepository>());
        var outsideSlot = new TimeSlot(new TimeOnly(18, 0), new TimeOnly(19, 0));

        var result = await sut.BookAsync(CreateRequest(member, outsideSlot));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task BookAsync_ConflictingAppointment_Fails()
    {
        var member = CreateStaffMember();
        var existing = new ServiceAppointment(Guid.NewGuid(), member.Id, Guid.NewGuid(), Haircut, TestDate, BookedSlot);
        existing.Confirm();

        var staffRepo = new Mock<IStaffMemberRepository>();
        var aptRepo = new Mock<IServiceAppointmentRepository>();
        staffRepo.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        aptRepo.Setup(r => r.GetByStaffMemberAndDateAsync(member.Id, TestDate, It.IsAny<CancellationToken>()))
               .ReturnsAsync([existing]);

        var sut = new ServiceAppointmentService(staffRepo.Object, aptRepo.Object);
        var result = await sut.BookAsync(CreateRequest(member, BookedSlot));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task BookAsync_WithCalendarConnector_AttachesExternalId()
    {
        var member = CreateStaffMember();
        var staffRepo = new Mock<IStaffMemberRepository>();
        var aptRepo = new Mock<IServiceAppointmentRepository>();
        var calendar = new Mock<ICalendarConnector>();
        staffRepo.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        aptRepo.Setup(r => r.GetByStaffMemberAndDateAsync(member.Id, TestDate, It.IsAny<CancellationToken>()))
               .ReturnsAsync([]);
        calendar.Setup(c => c.CreateEventAsync(It.IsAny<CalendarEventRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok("ext-id-abc"));

        var sut = new ServiceAppointmentService(staffRepo.Object, aptRepo.Object, calendar.Object);
        var result = await sut.BookAsync(CreateRequest(member, BookedSlot));

        Assert.True(result.IsSuccess);
        Assert.Equal("ext-id-abc", result.Value!.ExternalCalendarEventId);
    }

    [Fact]
    public async Task GetAvailableSlotsAsync_FiltersBookedSlots()
    {
        var member = CreateStaffMember();
        var existing = new ServiceAppointment(Guid.NewGuid(), member.Id, Guid.NewGuid(), Haircut, TestDate, BookedSlot);
        existing.Confirm();

        var staffRepo = new Mock<IStaffMemberRepository>();
        var aptRepo = new Mock<IServiceAppointmentRepository>();
        staffRepo.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        aptRepo.Setup(r => r.GetByStaffMemberAndDateAsync(member.Id, TestDate, It.IsAny<CancellationToken>()))
               .ReturnsAsync([existing]);

        var sut = new ServiceAppointmentService(staffRepo.Object, aptRepo.Object);
        var result = await sut.GetAvailableSlotsAsync(member.Id, TestDate);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value!, s => s == BookedSlot);
    }

    [Fact]
    public async Task CancelAsync_ExistingAppointment_Succeeds()
    {
        var member = CreateStaffMember();
        var apt = new ServiceAppointment(Guid.NewGuid(), member.Id, Guid.NewGuid(), Haircut, TestDate, BookedSlot);
        apt.Confirm();

        var aptRepo = new Mock<IServiceAppointmentRepository>();
        aptRepo.Setup(r => r.GetByIdAsync(apt.Id, It.IsAny<CancellationToken>())).ReturnsAsync(apt);

        var sut = new ServiceAppointmentService(Mock.Of<IStaffMemberRepository>(), aptRepo.Object);
        var result = await sut.CancelAsync(apt.Id);

        Assert.True(result.IsSuccess);
        aptRepo.Verify(r => r.SaveAsync(apt, It.IsAny<CancellationToken>()), Times.Once);
    }
}
