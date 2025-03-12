using Core;
using Core.Models;

namespace Events;

[Event(QueueNames.Board.Created)]
public record BoardCreatedEvent(
    string BoardId,
    string Name,
    string Owner,
    DateTimeOffset CreatedAt,
    List<BoardSection>? Sections) : Event(CreatedAt);