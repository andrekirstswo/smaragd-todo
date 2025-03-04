using System.Text.Json;
using Core;
using Core.Models;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace WebUI.Infrastructure;

public interface ITokenProvider
{
    Task<Result<Token?, Error>> GetTokenAsync(CancellationToken cancellationToken = default);
    Task<Result<string, Error>> GetTokenAsStringAsync(CancellationToken cancellationToken = default);
    Task SaveTokenAsStringAsync(Token token, CancellationToken cancellationToken = default);
    Task DeleteTokenAsync(CancellationToken cancellationToken = default);
}

public class TokenProvider : ITokenProvider
{
    public static readonly Error KeyNotFoundError = new Error("TOKEN_PROVIDER_KEY_NOT_FOUND", $"Key {Core.Constants.Token.Key} not found");

    private readonly ILocalStorageService _localStorageService;

    public TokenProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<Result<Token?, Error>> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var tokenString = await _localStorageService.GetItemAsStringAsync(Core.Constants.Token.Key);

        if (string.IsNullOrEmpty(tokenString))
        {
            return KeyNotFoundError;
        }

        return JsonSerializer.Deserialize<Token>(tokenString);
    }

    public async Task<Result<string, Error>> GetTokenAsStringAsync(CancellationToken cancellationToken = default)
    {
        var tokenString = await _localStorageService.GetItemAsStringAsync(Core.Constants.Token.Key);

        if (string.IsNullOrEmpty(tokenString))
        {
            return KeyNotFoundError;
        }

        return tokenString;
    }

    public async Task SaveTokenAsStringAsync(Token token, CancellationToken cancellationToken = default)
    {
        await _localStorageService.SaveAsStringAsync(Core.Constants.Token.Key, JsonSerializer.Serialize(token));
    }

    public Task DeleteTokenAsync(CancellationToken cancellationToken = default) => _localStorageService.DeleteItemAsync(Core.Constants.Token.Key);
}