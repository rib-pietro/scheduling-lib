using SchedulingLib.Core.Primitives;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Services.Tests.Entities;

public class StaffMemberTests
{
    private static WeeklySchedule EmptySchedule() => new WeeklySchedule([]);

    private static StaffMember CreateMember() =>
        new StaffMember(Guid.NewGuid(), "Alice", "alice@example.com", EmptySchedule());

    [Fact]
    public void AddService_NewService_Succeeds()
    {
        var member = CreateMember();
        var service = new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(60));

        var result = member.AddService(service);

        Assert.True(result.IsSuccess);
        Assert.Single(member.OfferedServices);
    }

    [Fact]
    public void AddService_DuplicateId_Fails()
    {
        var member = CreateMember();
        var service = new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(60));
        member.AddService(service);

        var result = member.AddService(service);

        Assert.False(result.IsSuccess);
        Assert.Single(member.OfferedServices);
    }

    [Fact]
    public void RemoveService_ExistingService_Succeeds()
    {
        var member = CreateMember();
        var service = new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(60));
        member.AddService(service);

        var result = member.RemoveService(service.Id);

        Assert.True(result.IsSuccess);
        Assert.Empty(member.OfferedServices);
    }

    [Fact]
    public void RemoveService_NonExistentService_Fails()
    {
        var member = CreateMember();

        var result = member.RemoveService(Guid.NewGuid());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void UpdateSchedule_ReplacesSchedule()
    {
        var member = CreateMember();
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var newSchedule = new WeeklySchedule([new DayOfWeekSchedule(DayOfWeek.Monday, [slot])]);

        member.UpdateSchedule(newSchedule);

        Assert.True(member.Schedule.ContainsDay(new DateOnly(2025, 6, 2)));
    }
}
