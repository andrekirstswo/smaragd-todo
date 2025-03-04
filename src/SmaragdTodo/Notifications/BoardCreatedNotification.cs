namespace Notifications;

public record BoardCreatedNotification(string BoardId, string Name, string Owner) : Notification;