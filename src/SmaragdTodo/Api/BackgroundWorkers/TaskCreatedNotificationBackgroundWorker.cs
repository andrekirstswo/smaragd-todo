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
            QueueNames.Task.CreatedNotification)
    {
    }

    protected override async Task Notify(TaskCreatedNotification notification, CancellationToken cancellationToken = default)
    {
        foreach (var assignee in notification.AssignedTo)
        {
            await HubContext.Clients.User(assignee).ReceiveTaskCreatedNotification(notification);
        }
    }
}