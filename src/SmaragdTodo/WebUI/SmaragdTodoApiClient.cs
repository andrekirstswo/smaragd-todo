using System.Net.Http.Json;
using Core;
using Core.Models;
using ErrorHandling;

namespace WebUI;

public class SmaragdTodoApiClient
{
    private readonly HttpClient _httpClient;

    public SmaragdTodoApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Token> GetAuthenticationTokenForUserId(string userId, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<Token>($"api/Authentication/token/{userId}", cancellationToken) ?? throw new NotSupportedException();
    }

    public Task<string> GetAuthenticationUrl(CancellationToken cancellationToken = default)
    {
        return _httpClient.GetStringAsync("api/Authentication", cancellationToken);
    }

    [Obsolete]
    public async Task<Result<CreateTaskResponseDto, Error>> CreateTaskAsync(string boardId, CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/board/{boardId}/task", dto, cancellationToken);

        return await HandleCreateAsync<CreateTaskResponseDto>(response, cancellationToken);
    }

    [Obsolete]
    private static async Task<TResult> HandleCreateAsync<TResult>(HttpResponseMessage response, CancellationToken cancellationToken = default)
        where TResult : new()
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<TResult>(cancellationToken) ?? new TResult();
        }

        throw new NotImplementedException();
    }
}