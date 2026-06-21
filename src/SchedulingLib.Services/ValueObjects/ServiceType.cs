namespace SchedulingLib.Services.ValueObjects;

/// <summary>
/// Describes a type of service offered by a <see cref="Entities.StaffMember"/>.
/// </summary>
public record ServiceType(Guid Id, string Name, decimal Price, TimeSpan Duration);
