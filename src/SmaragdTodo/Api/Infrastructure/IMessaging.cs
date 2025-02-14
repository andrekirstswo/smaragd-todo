using Events;

namespace Api.Infrastructure;

public interface IMessaging
{
    public Task PrepareAndSendMessageAsync<TEvent>(TEvent @event, Dictionary<string, object> additionalApplicationProperties, CancellationToken cancellationToken = default)
        where TEvent : Event;
}