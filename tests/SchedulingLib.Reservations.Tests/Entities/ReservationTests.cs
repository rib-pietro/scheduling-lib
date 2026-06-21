using SchedulingLib.Core.Primitives;
using SchedulingLib.Reservations.Entities;
using SchedulingLib.Reservations.Enums;

namespace SchedulingLib.Reservations.Tests.Entities;

public class ReservationTests
{
    private static Reservation CreatePending() => new(
        Guid.NewGuid(),
        "Seaside Villa",
        Guid.NewGuid(),
        Guid.NewGuid(),
        new DateRange(new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 7)),
        null);

    [Fact]
    public void NewReservation_IsPendingConfirmation()
    {
        var r = CreatePending();
        Assert.Equal(ReservationStatus.PendingConfirmation, r.Status);
    }

    [Fact]
    public void Confirm_FromPending_Succeeds()
    {
        var r = CreatePending();
        var result = r.Confirm();
        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Confirmed, r.Status);
    }

    [Fact]
    public void Confirm_NotFromPending_Fails()
    {
        var r = CreatePending();
        r.Confirm();
        var result = r.Confirm();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void CheckIn_FromConfirmed_Succeeds()
    {
        var r = CreatePending();
        r.Confirm();
        var result = r.CheckIn();
        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.CheckedIn, r.Status);
    }

    [Fact]
    public void CheckIn_NotFromConfirmed_Fails()
    {
        var r = CreatePending();
        var result = r.CheckIn();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void CheckOut_FromCheckedIn_Succeeds()
    {
        var r = CreatePending();
        r.Confirm();
        r.CheckIn();
        var result = r.CheckOut();
        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.CheckedOut, r.Status);
    }

    [Fact]
    public void CheckOut_NotFromCheckedIn_Fails()
    {
        var r = CreatePending();
        r.Confirm();
        var result = r.CheckOut();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Cancel_FromPending_Succeeds()
    {
        var r = CreatePending();
        var result = r.Cancel();
        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Cancelled, r.Status);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_Fails()
    {
        var r = CreatePending();
        r.Cancel();
        var result = r.Cancel();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Cancel_AfterCheckOut_Fails()
    {
        var r = CreatePending();
        r.Confirm();
        r.CheckIn();
        r.CheckOut();
        var result = r.Cancel();
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void AttachExternalId_SetsId()
    {
        var r = CreatePending();
        r.AttachExternalId("google-cal-456");
        Assert.Equal("google-cal-456", r.ExternalCalendarEventId);
    }
}
