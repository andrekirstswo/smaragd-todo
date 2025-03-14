using Core;
using Core.Models;

namespace Notifications;

[Notification(QueueNames.Board.CreatedNotification)]
public class BoardCreatedNotification : Notification
{
    public BoardCreatedNotification(string boardId, string name, string owner, List<BoardSection> sections)
    {
        BoardId = boardId;
        Name = name;
        Owner = owner;
        Sections = sections;
    }

    public string BoardId { get; init; }
    public string Name { get; init; }
    public string Owner { get; init; }
    public List<BoardSection> Sections { get; init; }
}