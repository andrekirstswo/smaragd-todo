using Api.Database;
using Api.Infrastructure;
using Core.Database.Models;
using Core.Infrastructure;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly SmaragdTodoContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly HttpContext? _httpContext;
    private readonly GoogleAuthenticationOptions _options;

    public AuthenticationController(
        SmaragdTodoContext context,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor,
        IOptions<GoogleAuthenticationOptions> options)
    {
        _options = options.Value;
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _httpContext = httpContextAccessor.HttpContext;
    }

    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_httpContext);
        ArgumentException.ThrowIfNullOrEmpty(request.Token);

        var token = request.Token;

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new [] { _options.ClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

        if (!payload.EmailVerified)
        {
            return Unauthorized(KnownErrors.Authentication.EmailNotVerified);
        }

        var email = payload.Email.ToLower();

        var user = await _context.Users
            .WithPartitionKey(email)
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            return Ok(new
            {
                UserId = payload.Subject
            });
        }

        var id = Guid.NewGuid().ToString();
        var userEntity = new User
        {
            UserId = id,
            id = id,
            CreatedAt = _dateTimeProvider.UtcNow,
            Email = email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            Picture = payload.Picture
        };

        await _context.Users.AddAsync(userEntity, cancellationToken);
        var rowsAffected = await _context.SaveChangesAsync(cancellationToken);

        if (rowsAffected == 1)
        {
            return Ok(new
            {
                UserId = payload.Subject
            });
        }

        return Unauthorized();
    }
}

public class LoginRequest
{
    public string? Token { get; set; }
}
