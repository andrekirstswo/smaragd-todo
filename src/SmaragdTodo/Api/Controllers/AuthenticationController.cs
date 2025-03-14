using System.Text.Json;
using Api.Database;
using Api.Services;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IGoogleAuthorization _googleAuthorization;
    private readonly CredentialRepository _credentialRepository;

    public AuthenticationController(
        IGoogleAuthorization googleAuthorization,
        CredentialRepository credentialRepository)
    {
        _googleAuthorization = googleAuthorization;
        _credentialRepository = credentialRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Authorize() => Ok(_googleAuthorization.GetAuthorizationUrl());

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(string code, CancellationToken cancellationToken = default)
    {
        var userCredential = await _googleAuthorization.ExchangeCodeForToken(code, cancellationToken);
        var credential = await _credentialRepository.GetByAccessToken(userCredential.Token.AccessToken, cancellationToken);
        
        ArgumentNullException.ThrowIfNull(credential);

        var userId = credential.UserId;

        // TODO Remove after test
        //var user = await _dbContext.Credentials
        //    .Where(c => c.AccessToken == userCredential.Token.AccessToken)
        //    .FirstOrDefaultAsync(cancellationToken);
        
        ArgumentNullException.ThrowIfNull(userId);

        // TODO refactor
        return Redirect($"https://localhost:7287/connect/{userId}");
    }

    [HttpGet("token/{userId}")]
    [ProducesResponseType(typeof(Token), 200)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAccessToken(string userId, CancellationToken cancellationToken = default)
    {
        var credential = await _credentialRepository.GetByUserId(userId, cancellationToken);

        if (credential is null)
        {
            return Unauthorized();
        }

        ArgumentNullException.ThrowIfNull(credential);
        ArgumentException.ThrowIfNullOrEmpty(credential.AccessToken);

        var response = JsonSerializer.Serialize(new Token(credential.AccessToken, credential.UserId));

        return Ok(response);
    }
}