using SchedulingLib.Services.Entities;

namespace SchedulingLib.Services.Interfaces;

/// <summary>
/// Persistence contract for <see cref="ServiceAppointment"/> entities.
/// Implement this in your infrastructure layer (EF Core, Dapper, etc.).
/// </summary>
public interface IServiceAppointmentRepository
{
    /// <summary>Returns the appointment with <paramref name="id"/>, or null if not found.</summary>
    Task<ServiceAppointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists a new or updated <see cref="ServiceAppointment"/>.</summary>
    Task SaveAsync(ServiceAppointment appointment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all appointments for <paramref name="staffMemberId"/> on <paramref name="date"/>.
    /// </summary>
    Task<IReadOnlyList<ServiceAppointment>> GetByStaffMemberAndDateAsync(Guid staffMemberId, DateOnly date, CancellationToken cancellationToken = default);
}
