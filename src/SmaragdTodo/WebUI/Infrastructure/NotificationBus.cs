using Notifications;

namespace WebUI.Infrastructure;

public class NotificationBus
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<T>(Func<T, Task> asyncHandler)
        where T : Notification
    {
        var type = typeof(T);

        if (!_subscribers.TryGetValue(type, out var subscribtion))
        {
            subscribtion = new List<Delegate>();
            _subscribers[type] = subscribtion;
        }

        subscribtion.Add(asyncHandler);
    }

    public void Unsubscribe<T>(Func<T, Task> asyncHandler)
        where T : Notification
    {
        var type = typeof(T);
        if (_subscribers.TryGetValue(type, out var subscribtion))
        {
            subscribtion.Remove(asyncHandler);
        }
    }

    public async Task Publish<T>(T notification)
        where T : Notification
    {
        var type = typeof(T);
        
        if (_subscribers.TryGetValue(type, out var subscribtion))
        {
            var tasks = subscribtion
                .OfType<Func<T, Task>>()
                .Select(handler => handler(notification));

            await Task.WhenAll(tasks);
        }
    }
}