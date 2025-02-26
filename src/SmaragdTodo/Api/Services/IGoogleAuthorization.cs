using System.IdentityModel.Tokens.Jwt;
using Api.Database;
using Core.Database.Models;
using Core.Infrastructure;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Api.Services;

public interface IGoogleAuthorization
{
    string GetAuthorizationUrl();
    Task<UserCredential> ExchangeCodeForToken(string code, CancellationToken cancellationToken = default);
    Task<UserCredential> ValidateToken(string accessToken, CancellationToken cancellationToken = default);
}

public class GoogleAuthorization : IGoogleAuthorization
{
    private readonly SmaragdTodoContext _dbContext;
    private readonly IGoogleAuthHelper _googleAuthHelper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly string? _redurectUri;

    public GoogleAuthorization(
        SmaragdTodoContext dbContext,
        IGoogleAuthHelper googleAuthHelper,
        IConfiguration configuration,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _googleAuthHelper = googleAuthHelper;
        _dateTimeProvider = dateTimeProvider;
        _redurectUri = configuration["Authentication:Google:RedirectUri"];
    }

    public string GetAuthorizationUrl() =>
        new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = _googleAuthHelper.GetClientSecrets(),
                    Scopes = _googleAuthHelper.GetScopes(),
                    Prompt = OpenIdConnectPrompt.Consent
                })
            .CreateAuthorizationCodeRequest(_redurectUri)
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

        var token = await flow.ExchangeCodeForTokenAsync("user", code, _redurectUri, cancellationToken);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.IdToken);

        var userId = $"Google@{jwt.Payload["sub"] as string}";

        var user = await _dbContext.Users
            .WithPartitionKey("Google")
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        var email = jwt.Payload["email"] as string;

        ArgumentException.ThrowIfNullOrEmpty(email);

        if (user is null)
        {
            _dbContext.Users.Add(new User
            {
                AuthenticationProvider = "Google",
                Id = userId,
                Name = new Name
                {
                    LastName = jwt.Payload["family_name"] as string,
                    FirstName = jwt.Payload["given_name"] as string
                },
                Email = email,
                CreatedAt = _dateTimeProvider.UtcNow,
                Picture = jwt.Payload["picture"] as string
            });
        }
        else
        {
            user.Name = new Name
            {
                LastName = jwt.Payload["family_name"] as string,
                FirstName = jwt.Payload["given_name"] as string
            };

            user.Email = email;
            user.Picture = jwt.Payload["picture"] as string;

            _dbContext.Users.Update(user);
        }

        _dbContext.Credentials.Add(new Credential
        {
            Id = Guid.NewGuid().ToString(),
            AccessToken = token.AccessToken,
            RefreshToken = token.RefreshToken,
            ExpiresInSeconds = token.ExpiresInSeconds,
            IdToken = token.IdToken,
            IssuedUtc = token.IssuedUtc,
            UserId = userId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        var userCredential = new UserCredential(flow, "user", token);
        return userCredential;
    }

    public async Task<UserCredential> ValidateToken(string accessToken, CancellationToken cancellationToken = default)
    {
        var credential = await _dbContext.Credentials
            .Where(u => u.AccessToken == accessToken)
            .FirstAsync(cancellationToken);

        ArgumentNullException.ThrowIfNull(credential);

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
            IssuedUtc = credential.IssuedUtc
        };

        return new UserCredential(flow, "user", tokenResponse);
    }
}