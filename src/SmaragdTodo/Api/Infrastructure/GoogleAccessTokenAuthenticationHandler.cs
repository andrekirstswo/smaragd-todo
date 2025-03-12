using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Api.Services;
using Core.Database.Models;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.CosmosRepository;
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
        IRepository<Credential> credentialRepository,
        IRepository<User> userRepository)
        : base(options, logger, encoder)
    {
        _googleAuthorization = googleAuthorization;
        _credentialRepository = credentialRepository;
        _userRepository = userRepository;
    }

    private readonly IGoogleAuthorization _googleAuthorization;
    private readonly IRepository<Credential> _credentialRepository;
    private readonly IRepository<User> _userRepository;

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
        var user = await GetUserAsync(userCredential.Token.AccessToken);
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

    private async Task<User?> GetUserAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var result = await _credentialRepository.GetAsync(p => p.AccessToken == accessToken, cancellationToken);
        var credentials = result.ToList();
        
        if (!credentials.Any())
        {
            return null;
        }

        var userId = credentials.First().UserId;

        var existsUser = await _userRepository.ExistsAsync(p => p.UserId == userId, cancellationToken: cancellationToken);

        if (!existsUser)
        {
            return null;
        }

        var users = await _userRepository.GetAsync(p => p.UserId == userId, cancellationToken: cancellationToken);

        return users.First();
    }
}