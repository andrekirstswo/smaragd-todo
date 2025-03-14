using Core;

namespace Notifications;

[Notification(QueueNames.Task.CreatedNotification)]
public class TaskCreatedNotification
{
    public string TaskId { get; set; } = default!;
    public string BoardId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public List<string> AssignedTo { get; set; } = new List<string>();
}