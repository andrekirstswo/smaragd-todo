using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NetcodeHub.Packages.Extensions.LocalStorage;
using WebUI.Infrastructure;

namespace WebUI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddCascadingAuthenticationState();
        builder.Services
            .AddHttpClient<SmaragdTodoApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7259");
            })
            .AddHttpMessageHandler<AuthHandler>();
        builder.Services.AddTransient<AuthHandler>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthState>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddNetcodeHubLocalStorageService();

        await builder.Build().RunAsync();
    }
}

public class AuthHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorageService;

    public AuthHandler(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorageService.GetItemAsStringAsync(Core.Constants.Token.Key);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}