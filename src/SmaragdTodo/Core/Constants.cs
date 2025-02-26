namespace Core;

public static class Constants
{
    public const string DatabaseName = "SmaragdTodo";

    public static class Request
    {
        public const string RequestId = "RequestId";
        public const string RequestSubmittedAt = "RequestSubmittedAt";
        public const string RequestStatusUrl = "RequestStatusUrl";
    }

    public static class Prefixes
    {
        public const string Bearer = "Bearer ";
    }

    public static class Token
    {
        public const string Scheme = "GoogleAccessToken";
        public const string Key = "credential";
    }
}