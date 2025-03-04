using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
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
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();

        builder.Services.AddMudServices();

        await builder.Build().RunAsync();
    }
}