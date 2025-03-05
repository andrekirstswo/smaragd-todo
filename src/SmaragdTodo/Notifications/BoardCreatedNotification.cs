using Core;

namespace Notifications;

[Notification(QueueNames.Board.CreatedNotification)]
public record BoardCreatedNotification(string BoardId, string Name, string Owner) : Notification;