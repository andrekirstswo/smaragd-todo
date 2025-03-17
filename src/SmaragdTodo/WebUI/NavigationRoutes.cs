namespace WebUI;

public static class NavigationRoutes
{
    public static class Boards
    {
        public const string Base = "/boards";
        public static string ById(string id) => $"{Base}/{id}";
        public const string Create = $"{Base}/create";
    }

    public static class Tasks
    {
        public static string Base(string boardId) => $"{Boards.ById(boardId)}/tasks";
        public static string ById(string boardId, string taskId) => $"{Base(boardId)}/tasks/{taskId}";
        public static string Create(string boardId) => $"{Base(boardId)}/create";
    }
}