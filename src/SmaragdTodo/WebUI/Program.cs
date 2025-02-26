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
        builder.Services.AddHttpClient(Constants.ApiClient, client =>
        {
            client.BaseAddress = new Uri("https://localhost:7259");
        });
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthState>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddNetcodeHubLocalStorageService();

        await builder.Build().RunAsync();
    }
}
