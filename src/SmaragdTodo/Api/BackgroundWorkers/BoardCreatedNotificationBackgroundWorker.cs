using Azure.Messaging.ServiceBus;
using Core;
using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api.BackgroundWorkers;

public class BoardCreatedNotificationBackgroundWorker : NotificationBackgroundWorkerBase<BoardCreatedNotification>
{
    public BoardCreatedNotificationBackgroundWorker(
        IHubContext<NotificationHub, INotificationHubClient> hubContext,
        ServiceBusClient serviceBusClient)
        : base(
            hubContext,
            serviceBusClient,
            QueueNames.Board.CreatedNotification)
    {
    }

    protected override async Task Notify(BoardCreatedNotification notification, CancellationToken cancellationToken = default)
    {
        await HubContext.Clients.User(notification.Owner).ReceiveBoardCreatedNotification(notification);
    }
}