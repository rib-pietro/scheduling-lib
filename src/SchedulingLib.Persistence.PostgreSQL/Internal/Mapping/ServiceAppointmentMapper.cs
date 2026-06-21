using System.Reflection;
using SchedulingLib.Core.Primitives;
using SchedulingLib.Persistence.PostgreSQL.Internal.Rows;
using SchedulingLib.Services.Entities;
using SchedulingLib.Services.Enums;

namespace SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;

internal static class ServiceAppointmentMapper
{
    // Backing field for get-only CreatedAt; set once in the domain constructor.
    private static readonly FieldInfo CreatedAtField =
        typeof(ServiceAppointment).GetField("<CreatedAt>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!;

    // Private setters for mutable state fields not reachable via the constructor.
    private static readonly PropertyInfo StatusProperty =
        typeof(ServiceAppointment).GetProperty(nameof(ServiceAppointment.Status))!;

    private static readonly PropertyInfo ExternalIdProperty =
        typeof(ServiceAppointment).GetProperty(nameof(ServiceAppointment.ExternalCalendarEventId))!;

    internal static ServiceAppointmentRow ToRow(ServiceAppointment appt) => new()
    {
        Id = appt.Id,
        Title = appt.Title,
        StaffMemberId = appt.StaffMemberId,
        ClientId = appt.ClientId,
        ServiceTypeId = appt.ServiceType.Id,
        Date = appt.Date,
        TimeSlotStart = appt.TimeSlot.Start,
        TimeSlotEnd = appt.TimeSlot.End,
        Status = appt.Status.ToString(),
        ExternalCalendarEventId = appt.ExternalCalendarEventId,
        CreatedAt = appt.CreatedAt,
    };

    // serviceTypeRow is loaded via join — service_appointments only stores the FK.
    internal static ServiceAppointment ToDomain(ServiceAppointmentRow row, ServiceTypeRow serviceTypeRow)
    {
        var serviceType = ServiceTypeMapper.ToDomain(serviceTypeRow);
        var timeSlot = new TimeSlot(row.TimeSlotStart, row.TimeSlotEnd);
        var appt = new ServiceAppointment(row.Id, row.StaffMemberId, row.ClientId, serviceType, row.Date, timeSlot);

        // Restore persisted state that differs from the constructor defaults.
        CreatedAtField.SetValue(appt, row.CreatedAt);
        StatusProperty.SetValue(appt, Enum.Parse<AppointmentStatus>(row.Status));
        if (row.ExternalCalendarEventId is not null)
            ExternalIdProperty.SetValue(appt, row.ExternalCalendarEventId);

        return appt;
    }
}
