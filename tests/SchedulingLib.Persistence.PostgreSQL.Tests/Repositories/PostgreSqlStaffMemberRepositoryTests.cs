using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Persistence.PostgreSQL.Tests.Repositories;

[Collection("PostgreSQL")]
public sealed class PostgreSqlStaffMemberRepositoryTests(PostgreSqlDatabaseFixture fixture)
{
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IStaffMemberRepository>();

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_ThenGetById_RoundTripsAllFields()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IStaffMemberRepository>();

        var staffId = Guid.NewGuid();
        var monday9to5 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var schedule = new WeeklySchedule([new DayOfWeekSchedule(DayOfWeek.Monday, [monday9to5])]);
        var staff = new StaffMember(staffId, "Alice Smith", "alice@example.com", schedule);
        var haircut = new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(60));
        staff.AddService(haircut);

        await repo.SaveAsync(staff);
        var retrieved = await repo.GetByIdAsync(staffId);

        Assert.NotNull(retrieved);
        Assert.Equal(staff.Id, retrieved.Id);
        Assert.Equal("Alice Smith", retrieved.Name);
        Assert.Equal("alice@example.com", retrieved.Email);

        Assert.Single(retrieved.Schedule.Days);
        Assert.Equal(DayOfWeek.Monday, retrieved.Schedule.Days[0].Day);
        Assert.Single(retrieved.Schedule.Days[0].TimeSlots);
        Assert.Equal(new TimeOnly(9, 0), retrieved.Schedule.Days[0].TimeSlots[0].Start);
        Assert.Equal(new TimeOnly(17, 0), retrieved.Schedule.Days[0].TimeSlots[0].End);

        Assert.Single(retrieved.OfferedServices);
        Assert.Equal(haircut.Id, retrieved.OfferedServices[0].Id);
        Assert.Equal("Haircut", retrieved.OfferedServices[0].Name);
        Assert.Equal(TimeSpan.FromMinutes(60), retrieved.OfferedServices[0].Duration);
    }

    [Fact]
    public async Task SaveAsync_WhenCalledTwice_UpdatesFields()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IStaffMemberRepository>();

        var staffId = Guid.NewGuid();
        var staff = new StaffMember(staffId, "Bob Jones", "bob@example.com", new WeeklySchedule([]));
        await repo.SaveAsync(staff);

        staff.UpdateSchedule(new WeeklySchedule([new DayOfWeekSchedule(DayOfWeek.Friday, [])]));
        await repo.SaveAsync(staff);

        var retrieved = await repo.GetByIdAsync(staffId);
        Assert.NotNull(retrieved);
        Assert.Single(retrieved.Schedule.Days);
        Assert.Equal(DayOfWeek.Friday, retrieved.Schedule.Days[0].Day);
    }
}
