using Azure.Messaging.ServiceBus;
using Core;
using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api.BackgroundWorkers;

public class TaskCreatedNotificationBackgroundWorker : NotificationBackgroundWorkerBase<TaskCreatedNotification>
{
    public TaskCreatedNotificationBackgroundWorker(
        IHubContext<NotificationHub, INotificationHubClient> hubContext,
        ServiceBusClient serviceBusClient)
        : base(
            hubContext,
            serviceBusClient,
            QueueNames.Task.CreatedNotification,
            async (context, notification) =>
            {
                foreach (var assignee in notification.AssignedTo)
                {
                    await context.Clients.User(assignee).ReceiveTaskCreatedNotification(notification);
                }
            })
    {
    }
}