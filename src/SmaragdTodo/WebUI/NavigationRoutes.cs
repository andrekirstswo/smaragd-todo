namespace WebUI;

public static class NavigationRoutes
{
    public static class Boards
    {
        public const string Base = "/boards";
        public static string ById(string id) => $"{Base}/{id}";
        public const string Create = $"{Base}/create";
    }
}