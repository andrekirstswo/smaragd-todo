using System.Security.Claims;
using System.Text.Encodings.Web;
using Api.Database;
using Api.Services;
using Core.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Api.Infrastructure;

public class GoogleAccessTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public GoogleAccessTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IGoogleAuthorization googleAuthorization,
        SmaragdTodoContext dbContext)
        : base(options, logger, encoder)
    {
        _googleAuthorization = googleAuthorization;
        _dbContext = dbContext;
    }

    private readonly IGoogleAuthorization _googleAuthorization;
    private readonly SmaragdTodoContext _dbContext;

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null)
        {
            return AuthenticateResult.NoResult();
        }

        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var header))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        string authenticationHeader = header!;

        if (!authenticationHeader.StartsWith(Core.Constants.Prefixes.Bearer, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        var accessToken = authenticationHeader[Core.Constants.Prefixes.Bearer.Length..].Trim();
        var userCredential = await _googleAuthorization.ValidateToken(accessToken);
        var user = await GetUser(userCredential.Token.AccessToken);
        if (user == null)
        {
            return AuthenticateResult.Fail("Invalid access token");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id)
        };

        var identity = new ClaimsIdentity(claims, Core.Constants.Token.Scheme);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Core.Constants.Token.Scheme);
        
        return AuthenticateResult.Success(authenticationTicket);
    }

    private async Task<User?> GetUser(string accessToken)
    {
        var userId = await _dbContext.Credentials
            .Where(c => c.AccessToken == accessToken)
            .Select(c => c.UserId)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            return null;
        }

        return await _dbContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }
}