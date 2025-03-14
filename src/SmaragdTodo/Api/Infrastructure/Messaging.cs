using System.Reflection;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Core.Infrastructure;
using Events;

namespace Api.Infrastructure;

public class Messaging : IMessaging
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IDateTimeProvider _dateTimeProvider;

    public Messaging(ServiceBusClient serviceBusClient, IDateTimeProvider dateTimeProvider)
    {
        _serviceBusClient = serviceBusClient;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task PrepareAndSendMessageAsync<TEvent>(TEvent @event, Dictionary<string, object> additionalApplicationProperties, CancellationToken cancellationToken = default)
        where TEvent : Event
    {
        var payload = JsonSerializer.Serialize(@event, DefaultJsonSerializerOptions.Value);
        var message = new ServiceBusMessage(payload);

        message.ApplicationProperties.Add(Constants.Request.RequestSubmittedAt, _dateTimeProvider.UtcNow);

        foreach (var additionalApplicationProperty in additionalApplicationProperties)
        {
            message.ApplicationProperties.Add(additionalApplicationProperty.Key, additionalApplicationProperty.Value);
        }

        var eventAttribute = @event.GetType().GetCustomAttribute<EventAttribute>();
        ArgumentNullException.ThrowIfNull(eventAttribute);

        var sender = _serviceBusClient.CreateSender(eventAttribute.QueueName);
        await sender.SendMessageAsync(message, cancellationToken);
    }
}