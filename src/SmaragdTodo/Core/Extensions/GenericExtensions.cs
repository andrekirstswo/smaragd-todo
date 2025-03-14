using System.Text.Json;

namespace Core.Extensions;

public static class GenericExtensions
{
    public static string ToJson<T>(this T value) => JsonSerializer.Serialize(value);
}