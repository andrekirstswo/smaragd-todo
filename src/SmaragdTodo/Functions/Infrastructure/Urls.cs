namespace Functions.Infrastructure;

public static class Urls
{
    private static string RequestStatusBase(string entity, string requestId)
        => $"http://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/request/{entity}/status/{requestId}";

    public static string BoardRequestStatus(string requestId)
        => RequestStatusBase("board", requestId);
}