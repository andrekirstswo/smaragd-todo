using System.Net.Http.Headers;
using Core.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NetcodeHub.Packages.Extensions.LocalStorage;
using SmaragdTodoAK.GraphQL;
using WebUI.Infrastructure;

namespace WebUI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped<GraphQLAuthorizationMessageHandler>();

        builder.Services
            .AddHttpClient(CryptoClient.ClientName, client => client.BaseAddress = new Uri("https://localhost:7259/graphql"))
            .AddHttpMessageHandler<GraphQLAuthorizationMessageHandler>();

        builder.Services.AddCryptoClient();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services
            .AddHttpClient<SmaragdTodoApiClient>(client =>
            {
                // TODO Config
                client.BaseAddress = new Uri("https://localhost:7259");
            })
            .AddHttpMessageHandler<AuthHandler>();
        builder.Services.AddTransient<AuthHandler>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthState>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddNetcodeHubLocalStorageService();
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<NotificationBus>();
        builder.Services.AddScoped<SignalRService>();

        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }

    //private static void ConfigureCryptoClient(IServiceProvider provider, HttpClient client)
    //{
    //    var tokenProvider = provider.GetRequiredService<ITokenProvider>();
    //    var token = tokenProvider.GetTokenAsync().GetAwaiter().GetResult();

    //    if (!token.IsSuccess)
    //    {
    //        throw new ArgumentException(token.Error!.Message);
    //    }

    //    client.BaseAddress = new Uri("https://localhost:7259/graphql");
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value.ToJson());
    //}
}

public class GraphQLAuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public GraphQLAuthorizationMessageHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();

        if (!token.IsSuccess)
        {
            throw new ArgumentException(token.Error!.Message);
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value.ToJson());

        return await base.SendAsync(request, cancellationToken);
    }
}