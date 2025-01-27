using Microsoft.AspNetCore.Http;

namespace Functions.Infrastructure.Extensions;

public static class HttpRequestQueryExtensions
{
    public static TEnum ParseEnum<TEnum>(this IQueryCollection query, string key, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(query[key].FirstOrDefault() ?? Enum.GetName(defaultValue) ?? throw new InvalidOperationException());
    }
}