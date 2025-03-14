using Core;

namespace Events;

[Event(QueueNames.Task.Created)]
public class TaskCreatedEvent : Event
{
    public TaskCreatedEvent(string boardId, string taskId, string title, List<string> assignedTo)
    {
        BoardId = boardId;
        TaskId = taskId;
        Title = title;
        AssignedTo = assignedTo;
    }

    public string BoardId { get; init; }
    public string TaskId { get; init; }
    public string Title { get; init; }
    public List<string> AssignedTo { get; init; }
}