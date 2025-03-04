namespace Notifications;

public interface INotificationHubClient
{
    Task ReceiveBoardCreatedNotification(BoardCreatedNotification notification);
}