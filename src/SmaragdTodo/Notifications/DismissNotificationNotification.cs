namespace Notifications;

public record DismissNotificationNotification(string BoardId, Notification SourceNotification) : Notification;