using Api.Database;
using Api.Infrastructure;
using Api.Services;
using Core;
using Core.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();
        builder.AddRedisDistributedCache("apicache");

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        builder.Services.AddSingleton<IMessaging, Messaging>();
        builder.Services
            .AddOptions<GoogleAuthenticationOptions>()
            .BindConfiguration("Authentication:Google");

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining<Program>();
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services
            .AddSignalR()
            .AddNamedAzureSignalR("signalr");
        builder.Services.AddSingleton<NotificationHub>();
        builder.AddAzureServiceBusClient(ConnectionNames.Messaging);

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Core.Constants.Token.Scheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, GoogleAccessTokenAuthenticationHandler>(Core.Constants.Token.Scheme, null)
            .AddGoogle(options =>
            {
                var section = builder.Configuration.GetSection("Authentication:Google");

                var clientId = section["ClientId"];
                var clientSecret = section["ClientSecret"];
                var redirectUri = section["RedirectUri"];

                ArgumentException.ThrowIfNullOrEmpty(clientId);
                ArgumentException.ThrowIfNullOrEmpty(clientSecret);
                ArgumentException.ThrowIfNullOrEmpty(redirectUri);

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.CallbackPath = $"/{redirectUri}";
            });
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IGoogleAuthHelper, GoogleAuthHelper>();
        builder.Services.AddScoped<IGoogleAuthorization, GoogleAuthorization>();
        
        builder.Services.AddHostedService<NotificationBackgroundWorker>();

        builder.Services.AddDbContext<SmaragdTodoContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString(ConnectionNames.CosmosDb);

            ArgumentException.ThrowIfNullOrEmpty(connectionString);

            options.UseCosmos(connectionString, Constants.DatabaseName);
        });

        builder.Services.AddAzureClients(factoryBuilder =>
        {
            var connectionString = builder.Configuration.GetConnectionString(ConnectionNames.Messaging);
            ArgumentException.ThrowIfNullOrEmpty(connectionString);
            factoryBuilder.AddServiceBusClient(connectionString);
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.DefaultFonts = false;
            });
        }

        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<NotificationHub>($"/{SignalRHubNames.Notifications}");

        app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmaragdTodoContext>();
        await dbContext.Database.EnsureCreatedAsync();

        await app.RunAsync();
    }
}
