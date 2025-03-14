using System.Text.Json;
using Core;
using Core.Models;
using ErrorHandling;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace WebUI.Infrastructure;

public interface ITokenProvider
{
    Task<Result<Token?, Error>> GetTokenAsync();
    Task<Result<string, Error>> GetTokenAsStringAsync();
    Task SaveTokenAsStringAsync(Token token);
    Task DeleteTokenAsync();
}

public class TokenProvider : ITokenProvider
{
    private readonly ILocalStorageService _localStorageService;

    public TokenProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<Result<Token?, Error>> GetTokenAsync()
    {
        var tokenString = await _localStorageService.GetItemAsStringAsync(Core.Constants.Token.Key);

        return string.IsNullOrEmpty(tokenString) 
            ? KnownErrors.Authentication.TokenProviderKeyNotFound
            : JsonSerializer.Deserialize<Token>(tokenString);
    }

    public async Task<Result<string, Error>> GetTokenAsStringAsync()
    {
        var tokenString = await _localStorageService.GetItemAsStringAsync(Core.Constants.Token.Key);

        return string.IsNullOrEmpty(tokenString) 
            ? KnownErrors.Authentication.TokenProviderKeyNotFound
            : tokenString;
    }

    public async Task SaveTokenAsStringAsync(Token token)
    {
        await _localStorageService.SaveAsStringAsync(Core.Constants.Token.Key, JsonSerializer.Serialize(token));
    }

    public Task DeleteTokenAsync() => _localStorageService.DeleteItemAsync(Core.Constants.Token.Key);
}