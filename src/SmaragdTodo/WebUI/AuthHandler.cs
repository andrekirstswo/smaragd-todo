using System.Net.Http.Headers;
using WebUI.Infrastructure;

namespace WebUI;

public class AuthHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var result = await _tokenProvider.GetTokenAsStringAsync();

        if (!result.IsSuccess)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var token = result.Value;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}