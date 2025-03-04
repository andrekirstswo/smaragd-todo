using System.Reflection;
using Events;
using Microsoft.AspNetCore.SignalR;

namespace Functions.Infrastructure.Extensions;

// ReSharper disable once InconsistentNaming
public static class IClientProxyExtensions
{
    //public static async Task SendEventAsync<TEvent>(this IClientProxy proxy, TEvent @event, CancellationToken cancellationToken = default)
    //{
    //    ArgumentNullException.ThrowIfNull(@event);

    //    var eventMethodAttribute = @event.GetType().GetCustomAttribute<EventMethodAttribute>();
    //    ArgumentNullException.ThrowIfNull(eventMethodAttribute);

    //    var methodName = eventMethodAttribute.MethodName;

    //    await proxy.SendAsync(methodName, @event, cancellationToken);
    //}
}