using System.Text.Json;
using Api.Database;
using Api.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IGoogleAuthorization _googleAuthorization;
    private readonly SmaragdTodoContext _dbContext;

    public AuthenticationController(
        IGoogleAuthorization googleAuthorization,
        SmaragdTodoContext dbContext)
    {
        _googleAuthorization = googleAuthorization;
        _dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Authorize() => Ok(_googleAuthorization.GetAuthorizationUrl());

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(string code, CancellationToken cancellationToken = default)
    {
        var userCredential = await _googleAuthorization.ExchangeCodeForToken(code, cancellationToken);
        var user = await _dbContext.Credentials
            .Where(c => c.AccessToken == userCredential.Token.AccessToken)
            .FirstOrDefaultAsync(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(user);

        return Redirect($"https://localhost:7287/connect/{user.UserId}");
    }

    [HttpGet("token/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAccessToken(string userId, CancellationToken cancellationToken = default)
    {
        var credential = await _dbContext.Credentials
            .Where(c => c.UserId == userId)
            .Select(u => new
            {
                u.Id,
                u.AccessToken
            })
            .FirstOrDefaultAsync(cancellationToken);

        ArgumentNullException.ThrowIfNull(credential);
        ArgumentException.ThrowIfNullOrEmpty(credential.AccessToken);

        var response = JsonSerializer.Serialize(new Token(credential.AccessToken, credential.Id));

        return Ok(response);
    }
}

public class LoginRequest
{
    public string? AccessToken { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public LoginResponseUser? User { get; set; }
}

public class LoginResponseUser
{
    public string Id { get; set; } = default!;
    public string? EMail { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}