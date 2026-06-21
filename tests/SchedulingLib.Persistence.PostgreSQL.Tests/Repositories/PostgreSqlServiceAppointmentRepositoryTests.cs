using Microsoft.Extensions.DependencyInjection;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Enums;
using SchedulingLib.Services.Interfaces;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Persistence.PostgreSQL.Tests.Repositories;

[Collection("PostgreSQL")]
public sealed class PostgreSqlServiceAppointmentRepositoryTests(PostgreSqlDatabaseFixture fixture)
{
    /// <summary>
    /// Saves a fresh ServiceType to the DB and returns a new ServiceAppointment referencing it.
    /// Required because service_appointments.service_type_id is a FK to service_types.
    /// </summary>
    private static async Task<ServiceAppointment> MakeAppointmentAsync(
        IServiceTypeRepository serviceTypeRepo,
        Guid? staffMemberId = null,
        DateOnly? date = null,
        TimeOnly? slotStart = null,
        TimeOnly? slotEnd = null)
    {
        var serviceType = new ServiceType(Guid.NewGuid(), "Haircut", 25.00m, TimeSpan.FromMinutes(30));
        await serviceTypeRepo.SaveAsync(serviceType);

        return new ServiceAppointment(
            Guid.NewGuid(),
            staffMemberId ?? Guid.NewGuid(),
            Guid.NewGuid(),
            serviceType,
            date ?? new DateOnly(2025, 6, 1),
            new TimeSlot(slotStart ?? new TimeOnly(10, 0), slotEnd ?? new TimeOnly(10, 30)));
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceAppointmentRepository>();

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task SaveAsync_ThenGetById_RoundTripsAllFields()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceAppointmentRepository>();
        var serviceTypeRepo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var appt = await MakeAppointmentAsync(serviceTypeRepo);
        await repo.SaveAsync(appt);
        var retrieved = await repo.GetByIdAsync(appt.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(appt.Id, retrieved.Id);
        Assert.Equal(appt.StaffMemberId, retrieved.StaffMemberId);
        Assert.Equal(appt.ClientId, retrieved.ClientId);
        Assert.Equal(appt.Date, retrieved.Date);
        Assert.Equal(appt.TimeSlot.Start, retrieved.TimeSlot.Start);
        Assert.Equal(appt.TimeSlot.End, retrieved.TimeSlot.End);
        Assert.Equal(appt.ServiceType.Id, retrieved.ServiceType.Id);
        Assert.Equal(appt.ServiceType.Name, retrieved.ServiceType.Name);
        Assert.Equal(appt.ServiceType.Price, retrieved.ServiceType.Price);
        Assert.Equal(appt.ServiceType.Duration, retrieved.ServiceType.Duration);
        Assert.Equal(AppointmentStatus.Pending, retrieved.Status);
        Assert.Null(retrieved.ExternalCalendarEventId);
    }

    [Fact]
    public async Task SaveAsync_AfterConfirm_PersistsUpdatedStatus()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceAppointmentRepository>();
        var serviceTypeRepo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var appt = await MakeAppointmentAsync(serviceTypeRepo);
        await repo.SaveAsync(appt);
        appt.Confirm();
        await repo.SaveAsync(appt);

        var retrieved = await repo.GetByIdAsync(appt.Id);
        Assert.Equal(AppointmentStatus.Confirmed, retrieved!.Status);
    }

    [Fact]
    public async Task SaveAsync_AfterAttachExternalId_PersistsExternalId()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceAppointmentRepository>();
        var serviceTypeRepo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var appt = await MakeAppointmentAsync(serviceTypeRepo);
        await repo.SaveAsync(appt);
        appt.AttachExternalId("gcal-event-abc123");
        await repo.SaveAsync(appt);

        var retrieved = await repo.GetByIdAsync(appt.Id);
        Assert.Equal("gcal-event-abc123", retrieved!.ExternalCalendarEventId);
    }

    [Fact]
    public async Task GetByStaffMemberAndDateAsync_ReturnsOnlyMatchingAppointments()
    {
        using var scope = fixture.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IServiceAppointmentRepository>();
        var serviceTypeRepo = scope.ServiceProvider.GetRequiredService<IServiceTypeRepository>();

        var staffId = Guid.NewGuid();
        var targetDate = new DateOnly(2025, 8, 15);

        var match1 = await MakeAppointmentAsync(serviceTypeRepo, staffMemberId: staffId, date: targetDate,
            slotStart: new TimeOnly(9, 0), slotEnd: new TimeOnly(9, 30));
        var match2 = await MakeAppointmentAsync(serviceTypeRepo, staffMemberId: staffId, date: targetDate,
            slotStart: new TimeOnly(10, 0), slotEnd: new TimeOnly(10, 30));
        var otherDate = await MakeAppointmentAsync(serviceTypeRepo, staffMemberId: staffId,
            date: targetDate.AddDays(1), slotStart: new TimeOnly(9, 0), slotEnd: new TimeOnly(9, 30));
        var otherStaff = await MakeAppointmentAsync(serviceTypeRepo, date: targetDate,
            slotStart: new TimeOnly(9, 0), slotEnd: new TimeOnly(9, 30));

        await repo.SaveAsync(match1);
        await repo.SaveAsync(match2);
        await repo.SaveAsync(otherDate);
        await repo.SaveAsync(otherStaff);

        var results = await repo.GetByStaffMemberAndDateAsync(staffId, targetDate);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, a => a.Id == match1.Id);
        Assert.Contains(results, a => a.Id == match2.Id);
    }
}
