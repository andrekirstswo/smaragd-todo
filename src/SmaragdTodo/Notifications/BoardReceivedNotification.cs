using Core.Models;

namespace Notifications;

public class BoardReceivedNotification : Notification
{
    public BoardReceivedNotification(GetBoardByIdDto board)
    {
        Board = board;
    }

    public GetBoardByIdDto Board { get; init; }
}