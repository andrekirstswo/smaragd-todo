using Azure.Messaging.ServiceBus;
using Core.Extensions;
using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api.BackgroundWorkers;

public abstract class NotificationBackgroundWorkerBase<TNotification> : BackgroundService
    where TNotification : Notification
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _queueName;

    protected NotificationBackgroundWorkerBase(
        IHubContext<NotificationHub, INotificationHubClient> hubContext,
        ServiceBusClient serviceBusClient,
        string queueName)
    {
        HubContext = hubContext;
        _serviceBusClient = serviceBusClient;
        _queueName = queueName;
    }

    protected IHubContext<NotificationHub, INotificationHubClient> HubContext { get; init; }

    protected abstract Task Notify(TNotification notification, CancellationToken cancellationToken = default);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiver = _serviceBusClient.CreateReceiver(_queueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await receiver.ReceiveMessageAsync(cancellationToken: stoppingToken);

            if (message is null)
            {
                await Task.Delay(500, stoppingToken);
                continue;
            }

            var notification = await message.Body.ToObjectFromJsonAsync<TNotification>(stoppingToken);

            if (notification is null)
            {
                continue;
            }

            await Notify(notification, stoppingToken);

            await receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }
}