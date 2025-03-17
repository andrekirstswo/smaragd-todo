using System.Security.Claims;

namespace Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier)?.Value;

        ArgumentException.ThrowIfNullOrEmpty(userId);

        return userId;
    }
}