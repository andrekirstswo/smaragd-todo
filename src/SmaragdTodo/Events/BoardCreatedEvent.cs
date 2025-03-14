using Core;
using Core.Models;

namespace Events;

[Event(QueueNames.Board.Created)]
public class BoardCreatedEvent : Event
{
    public BoardCreatedEvent(string boardId,
        string name,
        string owner,
        List<BoardSection>? sections)
    {
        BoardId = boardId;
        Name = name;
        Owner = owner;
        Sections = sections;
    }

    public string BoardId { get; init; }
    public string Name { get; init; }
    public string Owner { get; init; }
    public List<BoardSection>? Sections { get; init; }
}