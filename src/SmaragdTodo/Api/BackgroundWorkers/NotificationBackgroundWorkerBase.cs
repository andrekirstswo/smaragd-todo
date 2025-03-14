using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api.BackgroundWorkers;

public abstract class NotificationBackgroundWorkerBase<TNotification> : BackgroundService
    where TNotification : Notification
{
    private readonly IHubContext<NotificationHub, INotificationHubClient> _hubContext;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _queueName;
    private readonly Func<IHubContext<NotificationHub, INotificationHubClient>, TNotification, Task> _signalr;

    protected NotificationBackgroundWorkerBase(
        IHubContext<NotificationHub, INotificationHubClient> hubContext,
        ServiceBusClient serviceBusClient,
        string queueName,
        Func<IHubContext<NotificationHub, INotificationHubClient>, TNotification, Task> signalr)
    {
        _hubContext = hubContext;
        _serviceBusClient = serviceBusClient;
        _queueName = queueName;
        _signalr = signalr;
    }

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

            var notification = await JsonSerializer.DeserializeAsync<TNotification>(message.Body.ToStream(), cancellationToken: stoppingToken);

            if (notification is null)
            {
                continue;
            }

            await _signalr(_hubContext, notification);

            await receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }
}