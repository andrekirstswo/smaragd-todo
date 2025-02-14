using System.Security.Claims;

namespace Api.Infrastructure;

/// <summary>
/// Provides extension methods for the ClaimsPrincipal class to retrieve claims by their type.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Tries to get the value of a claim by its type.
    /// </summary>
    /// <param name="claimsPrincipal">The ClaimsPrincipal instance to search for the claim.</param>
    /// <param name="claimType">The type of the claim to search for.</param>
    /// <param name="value">When this method returns, contains the value of the claim if found; otherwise, an empty string.</param>
    /// <returns>true if the claim is found; otherwise, false.</returns>
    public static bool TryGetClaimByName(this ClaimsPrincipal claimsPrincipal, string claimType, out string value)
    {
        // Search for the claim with the specified type in the ClaimsPrincipal instance
        var found = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == claimType);

        // If the claim is not found, set the output value to an empty string and return false
        if (found == null)
        {
            value = string.Empty;
            return false;
        }

        // If the claim is found, set the output value to the claim's value and return true
        value = found.Value;
        return true;
    }
}
