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
            QueueNames.Board.CreatedNotification,
            async (context, notification) =>
            {
                await context.Clients.User(notification.Owner).ReceiveBoardCreatedNotification(notification);
            })
    {
    }
}