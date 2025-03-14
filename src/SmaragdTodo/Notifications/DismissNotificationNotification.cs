// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Notifications;

public class DismissNotificationNotification : Notification
{
    public DismissNotificationNotification(string boardId, Notification sourceNotification)
    {
        BoardId = boardId;
        SourceNotification = sourceNotification;
    }

    public string BoardId { get; init; }
    public Notification SourceNotification { get; init; }
}