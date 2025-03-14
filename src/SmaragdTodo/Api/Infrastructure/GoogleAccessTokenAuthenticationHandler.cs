using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Api.Database;
using Api.Services;
using Core;
using Core.Database.Models;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        CredentialRepository credentialRepository,
        UserRepository userRepository)
        : base(options, logger, encoder)
    {
        _googleAuthorization = googleAuthorization;
        _credentialRepository = credentialRepository;
        _userRepository = userRepository;
    }

    private readonly IGoogleAuthorization _googleAuthorization;
    private readonly CredentialRepository _credentialRepository;
    private readonly UserRepository _userRepository;

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
        var token = JsonSerializer.Deserialize<Token>(accessToken);
        
        ArgumentNullException.ThrowIfNull(token);
        
        var userCredential = await _googleAuthorization.ValidateToken(token);

        if (userCredential is not { Token: {} })
        {
            return AuthenticateResult.Fail("Invalid access token");
        }

        var user = await GetUserAsync(userCredential.Token.AccessToken);
        if (user == null)
        {
            return AuthenticateResult.Fail("Invalid access token");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId)
        };

        var identity = new ClaimsIdentity(claims, Core.Constants.Token.Scheme);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Core.Constants.Token.Scheme);
        
        return AuthenticateResult.Success(authenticationTicket);
    }

    private async Task<User?> GetUserAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.GetByAccessToken(accessToken, cancellationToken);

        return credential is null
            ? null
            : await _userRepository.GetByIdAsync(AuthenticationProviders.Google, credential.UserId, cancellationToken);
    }
}