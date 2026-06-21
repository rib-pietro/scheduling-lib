using SchedulingLib.Core.Primitives;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Enums;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Tests.Entities;

public class ServiceAppointmentTests
{
    private static ServiceAppointment CreatePending() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(60)),
        new DateOnly(2025, 6, 10),
        new TimeSlot(new TimeOnly(10, 0), new TimeOnly(11, 0)));

    [Fact]
    public void NewAppointment_IsPending()
    {
        var apt = CreatePending();
        Assert.Equal(AppointmentStatus.Pending, apt.Status);
    }

    [Fact]
    public void Confirm_FromPending_Succeeds()
    {
        var apt = CreatePending();
        var result = apt.Confirm();
        Assert.True(result.IsSuccess);
        Assert.Equal(AppointmentStatus.Confirmed, apt.Status);
    }

    [Fact]
    public void Confirm_FromConfirmed_Fails()
    {
        var apt = CreatePending();
        apt.Confirm();
        var result = apt.Confirm();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Cancel_FromPending_Succeeds()
    {
        var apt = CreatePending();
        var result = apt.Cancel();
        Assert.True(result.IsSuccess);
        Assert.Equal(AppointmentStatus.Cancelled, apt.Status);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_Fails()
    {
        var apt = CreatePending();
        apt.Cancel();
        var result = apt.Cancel();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Confirm_AfterCancelled_Fails()
    {
        var apt = CreatePending();
        apt.Cancel();
        var result = apt.Confirm();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AttachExternalId_SetsId()
    {
        var apt = CreatePending();
        apt.AttachExternalId("google-event-123");
        Assert.Equal("google-event-123", apt.ExternalCalendarEventId);
    }
}
