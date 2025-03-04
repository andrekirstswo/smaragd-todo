using Azure.Messaging.ServiceBus;
using Core;
using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api;

public class NotificationBackgroundWorker : BackgroundService
{
    private readonly IHubContext<NotificationHub, INotificationHubClient> _hubContext;
    private readonly ServiceBusClient _serviceBusClient;

    public NotificationBackgroundWorker(
        IHubContext<NotificationHub, INotificationHubClient> hubContext,
        ServiceBusClient serviceBusClient)
    {
        _hubContext = hubContext;
        _serviceBusClient = serviceBusClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiver = _serviceBusClient.CreateReceiver(QueueNames.Board.CreatedNotification);

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await receiver.ReceiveMessageAsync(cancellationToken: stoppingToken);

            if (message is null)
            {
                await Task.Delay(500, stoppingToken);
                continue;
            }

            var notification = message.Body.ToObjectFromJson<BoardCreatedNotification>();
            
            if (notification is null)
            {
                continue;
            }

            await _hubContext.Clients.User(notification.Owner).ReceiveBoardCreatedNotification(notification);

            await receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }
}