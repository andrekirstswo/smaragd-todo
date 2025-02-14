using Core;

namespace Api;

public static class KnownErrors
{
    public static class Authentication
    {
        public static readonly Error EmailNotVerified = new Error("EMAIL_NOT_VERIFIED", "Email not verified");
    }
}