using Api.Extensions;
using Core.Database.Models;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Middlewares;

public class CheckExistsUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IReadOnlyRepository<User> _userRepository;
    private readonly HybridCache _hybridCache;
    private readonly HttpContext? _httpContext;

    public CheckExistsUserMiddleware(
        RequestDelegate next,
        IHttpContextAccessor httpContextAccessor,
        IReadOnlyRepository<User> userRepository,
        HybridCache hybridCache)
    {
        _next = next;
        _userRepository = userRepository;
        _hybridCache = hybridCache;
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = _httpContext?.User.UserId();

        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

        var existsUser = await _hybridCache.GetOrCreateAsync(
            $"{nameof(CheckExistsUserMiddleware)}:UserId={userId}",
            async cancel => await _userRepository.ExistsAsync(p => p.UserId == userId, cancel));

        if (!existsUser)
        {
            throw new UnauthorizedAccessException();
        }
        
        await _next(context);
    }
}