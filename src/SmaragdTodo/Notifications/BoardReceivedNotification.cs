using Core.Models;

namespace Notifications;

public record BoardReceivedNotification(GetBoardByIdDto Board) : Notification;