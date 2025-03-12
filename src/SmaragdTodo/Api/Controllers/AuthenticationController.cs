using System.Linq.Expressions;
using System.Text.Json;
using Api.Services;
using Core.Database.Models;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CosmosRepository;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IGoogleAuthorization _googleAuthorization;
    private readonly IRepository<Credential> _credentialRepository;

    public AuthenticationController(
        IGoogleAuthorization googleAuthorization,
        IRepository<Credential> credentialRepository)
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

        var credentials = await _credentialRepository.GetAsync(p => p.AccessToken == userCredential.Token.AccessToken, cancellationToken);

        var list = credentials.ToList();
        if (!list.Any())
        {
            return BadRequest();
        }

        var userId = list.First().UserId;

        // TODO Remove after test
        //var user = await _dbContext.Credentials
        //    .Where(c => c.AccessToken == userCredential.Token.AccessToken)
        //    .FirstOrDefaultAsync(cancellationToken);
        
        ArgumentException.ThrowIfNullOrEmpty(userId);

        return Redirect($"https://localhost:7287/connect/{userId}");
    }

    [HttpGet("token/{userId}")]
    [ProducesResponseType(typeof(Token), 200)]
    [AllowAnonymous]
    public async Task<IActionResult> GetAccessToken(string userId, CancellationToken cancellationToken = default)
    {
        Expression<Func<Credential, bool>> expression = p => p.UserId == userId;

        var exists = await _credentialRepository.ExistsAsync(expression, cancellationToken: cancellationToken);

        if (!exists)
        {
            return Unauthorized();
        }

        var credentials = await _credentialRepository.GetAsync(expression, cancellationToken: cancellationToken);
        var credential = credentials.First();

        ArgumentNullException.ThrowIfNull(credential);
        ArgumentException.ThrowIfNullOrEmpty(credential.AccessToken);

        var response = JsonSerializer.Serialize(new Token(credential.AccessToken, credential.Id));

        return Ok(response);
    }
}

// TODO Remove?
public class LoginResponseUser
{
    public string Id { get; set; } = default!;
    public string? EMail { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}