using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Core;
using Core.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api;

public class BoardCreatedNotificationBackgroundWorker : BackgroundService
{
    private readonly IHubContext<NotificationHub, INotificationHubClient> _hubContext;
    private readonly ServiceBusClient _serviceBusClient;

    public BoardCreatedNotificationBackgroundWorker(
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

            var notification = await JsonSerializer.DeserializeAsync<BoardCreatedNotification>(
                message.Body.ToStream(),
                DefaultJsonSerializerOptions.Value,
                stoppingToken);
            
            if (notification is null)
            {
                continue;
            }

            await _hubContext.Clients.User(notification.Owner).ReceiveBoardCreatedNotification(notification);

            await receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }
}