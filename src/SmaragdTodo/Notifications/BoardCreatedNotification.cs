using Core;
using Core.Models;

namespace Notifications;

[Notification(QueueNames.Board.CreatedNotification)]
public record BoardCreatedNotification(
    string BoardId,
    string Name,
    string Owner,
    List<BoardSection> Sections) : Notification;