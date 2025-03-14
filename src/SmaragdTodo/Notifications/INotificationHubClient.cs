namespace Notifications;

public interface INotificationHubClient
{
    Task ReceiveBoardCreatedNotification(BoardCreatedNotification notification);
    Task ReceiveTaskCreatedNotification(TaskCreatedNotification notification);
}