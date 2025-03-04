using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace WebUI.Infrastructure;

public class 
    CustomAuthState : AuthenticationStateProvider
{
    private readonly ITokenProvider _tokenProvider;
    private ClaimsPrincipal _claimsPrincipal = new(new ClaimsIdentity());

    public CustomAuthState(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var result = await _tokenProvider.GetTokenAsync();

        if (!result.IsSuccess || result.Value is null)
        {
            return await Task.FromResult(new AuthenticationState(_claimsPrincipal));
        }

        var token = result.Value;

        ArgumentNullException.ThrowIfNull(token);

        _claimsPrincipal = SetClaimPrincipal(token.UserId);
        return await Task.FromResult(new AuthenticationState(_claimsPrincipal));
    }

    private static ClaimsPrincipal SetClaimPrincipal(string userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, Core.Constants.Token.Scheme));
    }

    public void NotifyAuthStateChanged() => NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_claimsPrincipal)));
}