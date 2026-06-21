using System.Text.Json;

namespace SchedulingLib.Persistence.PostgreSQL.Internal.Json;

internal static class SchedulingJsonOptions
{
    internal static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web)
    {
        Converters = { new TimeSpanTicksConverter() }
    };
}
