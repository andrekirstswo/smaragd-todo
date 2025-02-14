using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

public static class DateTimeProviderRegistration
{
    public static void AddDateTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}