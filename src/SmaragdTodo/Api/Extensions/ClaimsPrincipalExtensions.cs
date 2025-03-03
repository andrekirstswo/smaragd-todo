using System.Security.Claims;

namespace Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string UserId(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal.Claims.First(t => t.Type == ClaimTypes.NameIdentifier).Value;
    }
}