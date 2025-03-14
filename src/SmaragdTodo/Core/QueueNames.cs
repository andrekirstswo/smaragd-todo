namespace Core;

public static class QueueNames
{
    public static class Board
    {
        public const string Created = "board-created";
        public const string CreatedNotification = $"{Created}-notification";
    }

    public static class Task
    {
        public const string Created = "task-created";
        public const string CreatedNotification = $"{Created}-notification";
    }
}