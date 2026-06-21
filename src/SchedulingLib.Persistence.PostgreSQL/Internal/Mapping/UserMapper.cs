using SchedulingLib.Persistence.PostgreSQL.Internal.Rows;
using SchedulingLib.Users.Entities;

namespace SchedulingLib.Persistence.PostgreSQL.Internal.Mapping;

internal static class UserMapper
{
    public static User ToDomain(UserRow row) =>
        new(row.Id, row.Name, row.Email, row.Phone, row.CreatedAt);

    public static UserRow ToRow(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Phone = user.Phone,
        CreatedAt = user.CreatedAt
    };
}
