namespace Notifications;

[AttributeUsage(AttributeTargets.Class)]
public class NotificationAttribute : Attribute
{
    public string QueueName { get; }

    public NotificationAttribute(string queueName)
    {
        QueueName = queueName;
    }
}