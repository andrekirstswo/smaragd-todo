using System.Text.Json;

namespace Core.Infrastructure;

public static class DefaultJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Value = new JsonSerializerOptions(JsonSerializerOptions.Web);
}