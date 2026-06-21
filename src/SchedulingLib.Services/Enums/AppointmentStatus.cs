namespace SchedulingLib.Services.Enums;

/// <summary>
/// Lifecycle states of a <see cref="Entities.ServiceAppointment"/>.
/// </summary>
public enum AppointmentStatus
{
    /// <summary>The appointment has been requested but not yet confirmed.</summary>
    Pending,

    /// <summary>The appointment has been confirmed by the staff member.</summary>
    Confirmed,

    /// <summary>The appointment has been cancelled.</summary>
    Cancelled,
}
