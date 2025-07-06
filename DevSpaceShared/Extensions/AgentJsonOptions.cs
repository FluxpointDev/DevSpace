using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DevSpaceShared;

public static class AgentJsonOptions
{
    public static JsonSerializerOptions Options = new JsonSerializerOptions
    {
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        AllowTrailingCommas = true
    };

    public static JsonNode? FromObject<T>(T obj)
    {
        return JsonSerializer.SerializeToNode(obj, Options);
    }
}
