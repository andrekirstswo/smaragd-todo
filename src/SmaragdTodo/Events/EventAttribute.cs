namespace Events;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute
{
    public string QueueName { get; }

    public EventAttribute(string queueName)
    {
        QueueName = queueName;
    }
}