using SchedulingLib.Persistence.PostgreSQL.Internal.Rows;
using SchedulingLib.Services.ValueObjects;

namespace SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;

internal static class ServiceTypeMapper
{
    internal static ServiceTypeRow ToRow(ServiceType serviceType) => new()
    {
        Id = serviceType.Id,
        Name = serviceType.Name,
        Price = serviceType.Price,
        DurationTicks = serviceType.Duration.Ticks,
    };

    internal static ServiceType ToDomain(ServiceTypeRow row) =>
        new(row.Id, row.Name, row.Price, TimeSpan.FromTicks(row.DurationTicks));
}
