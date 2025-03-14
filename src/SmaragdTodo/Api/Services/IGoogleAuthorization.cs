using System.IdentityModel.Tokens.Jwt;
using Api.Database;
using Core.Database.Models;
using Core.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Api.Services;

public interface IGoogleAuthorization
{
    string GetAuthorizationUrl();
    Task<UserCredential> ExchangeCodeForToken(string code, CancellationToken cancellationToken = default);
    Task<UserCredential> ValidateToken(Token token, CancellationToken cancellationToken = default);
}

public class GoogleAuthorization : IGoogleAuthorization
{
    private readonly IGoogleAuthHelper _googleAuthHelper;
    private readonly CredentialRepository _credentialRepository;
    private readonly UserRepository _userRepository;
    private readonly HybridCache _hybridCache;
    private readonly string? _redirectUri;

    public GoogleAuthorization(
        IGoogleAuthHelper googleAuthHelper,
        IConfiguration configuration,
        CredentialRepository credentialRepository,
        UserRepository userRepository,
        HybridCache hybridCache)
    {
        _googleAuthHelper = googleAuthHelper;
        _credentialRepository = credentialRepository;
        _userRepository = userRepository;
        _hybridCache = hybridCache;
        _redirectUri = configuration["Authentication:Google:RedirectUri"];
    }

    public string GetAuthorizationUrl() =>
        new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = _googleAuthHelper.GetClientSecrets(),
                    Scopes = _googleAuthHelper.GetScopes(),
                    Prompt = OpenIdConnectPrompt.Consent
                })
            .CreateAuthorizationCodeRequest(_redirectUri)
            .Build()
            .ToString();

    public async Task<UserCredential> ExchangeCodeForToken(string code, CancellationToken cancellationToken = default)
    {
        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = _googleAuthHelper.GetClientSecrets(),
                Scopes = _googleAuthHelper.GetScopes()
            });

        var token = await flow.ExchangeCodeForTokenAsync("user", code, _redirectUri, cancellationToken);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.IdToken);

        var userId = $"Google@{jwt.Payload["sub"] as string}";

        var existsUser = await _userRepository.ExistsAsync("Google", userId, cancellationToken);

        var email = jwt.Payload["email"] as string;

        ArgumentException.ThrowIfNullOrEmpty(email);

        if (!existsUser)
        {
            await _userRepository.CreateAsync(new User
            {
                AuthenticationProvider = "Google",
                UserId = userId,
                Name = new Name
                {
                    LastName = jwt.Payload["family_name"] as string,
                    FirstName = jwt.Payload["given_name"] as string
                },
                Email = email,
                Picture = jwt.Payload["picture"] as string
            }, cancellationToken);
        }
        else
        {
            var user = await _userRepository.GetByIdAsync("Google", userId, cancellationToken);

            ArgumentNullException.ThrowIfNull(user);

            user.Name = new Name
            {
                LastName = jwt.Payload["family_name"] as string,
                FirstName = jwt.Payload["given_name"] as string
            };

            user.Email = email;
            user.Picture = jwt.Payload["picture"] as string;

            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        var credential = new Credential
        {
            CredentialId = Guid.CreateVersion7().ToString(),
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresInSeconds = token.ExpiresInSeconds,
            IdToken = token.IdToken,
            IssuedUtc = token.IssuedUtc,
            UserId = userId
        };
        await _hybridCache.SetAsync(
            CreateAccessTokenCacheKey(token.AccessToken),
            credential,
            DefaultHybridCacheEntryOptions(),
            cancellationToken: cancellationToken);

        await _credentialRepository.CreateAsync(credential, cancellationToken);

        return new UserCredential(flow, "user", token);
    }

    private static HybridCacheEntryOptions DefaultHybridCacheEntryOptions() =>
        new()
        {
            Expiration = TimeSpan.FromMinutes(1)
        };

    public async Task<UserCredential> ValidateToken(Token token, CancellationToken cancellationToken = default)
    {
        var accessToken = token.AccessToken;

        var cacheKey = CreateAccessTokenCacheKey(accessToken);
        var credential = await _hybridCache.GetOrCreateAsync(
            cacheKey,
            async cancel => await _credentialRepository.GetByAccessToken(accessToken, cancel),
            DefaultHybridCacheEntryOptions(),
            cancellationToken: cancellationToken);

        if (credential is null)
        {
            return null!;
        }

        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = _googleAuthHelper.GetClientSecrets(),
                Scopes = _googleAuthHelper.GetScopes()
            });

        var tokenResponse = new TokenResponse
        {
            AccessToken = credential.AccessToken,
            RefreshToken = credential.RefreshToken,
            ExpiresInSeconds = credential.ExpiresInSeconds,
            IdToken = credential.IdToken,
            IssuedUtc = credential.IssuedUtc.DateTime
        };

        return new UserCredential(flow, "user", tokenResponse);
    }

    private static string CreateAccessTokenCacheKey(string accessToken) => $"AccessToken:{accessToken}";
}