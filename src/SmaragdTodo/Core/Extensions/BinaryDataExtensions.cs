using System.Text.Json;

namespace Core.Extensions;

public static class BinaryDataExtensions
{
    public static ValueTask<T?> ToObjectFromJsonAsync<T>(this BinaryData binaryData, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(binaryData.ToStream(), cancellationToken: cancellationToken);
}