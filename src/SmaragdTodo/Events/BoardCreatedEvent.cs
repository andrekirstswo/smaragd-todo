using Core;

namespace Events;

[Event(QueueNames.Board.Created)]
public record BoardCreatedEvent(string BoardId, string Name, string Owner, DateTimeOffset CreatedAt) : Event(CreatedAt);