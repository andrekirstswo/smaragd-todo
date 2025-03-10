﻿using System.Net.Http.Json;
using Core.Models;

namespace WebUI;

public class SmaragdTodoApiClient
{
    private readonly HttpClient _httpClient;

    public SmaragdTodoApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<GetBoardsDto>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<GetBoardsDto>>("api/Board", cancellationToken) ?? new List<GetBoardsDto>();
    }

    public async Task<Token> GetAuthenticationTokenForUserId(string userId, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<Token>($"api/Authentication/token/{userId}", cancellationToken) ?? throw new NotSupportedException();
    }

    public Task<string> GetAuthenticationUrl(CancellationToken cancellationToken = default)
    {
        return _httpClient.GetStringAsync("api/Authentication", cancellationToken);
    }

    public async Task<CreateBoardResponseDto> CreateBoardAsync(CreateBoardDto dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Board", dto, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CreateBoardResponseDto>(cancellationToken) ?? new CreateBoardResponseDto();
        }

        throw new NotImplementedException();
    }
}