using System.Security.Claims;
using System.Text.Json;
using Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace WebUI.Infrastructure;

public class 
    CustomAuthState : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;

    private ClaimsPrincipal _claimsPrincipal = new(new ClaimsIdentity());

    public CustomAuthState(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var tokenString = await _localStorageService.GetItemAsStringAsync(Core.Constants.Token.Key);
        
        if (string.IsNullOrEmpty(tokenString))
        {
            return await Task.FromResult(new AuthenticationState(_claimsPrincipal));
        }

        var token = JsonSerializer.Deserialize<Token>(tokenString);

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