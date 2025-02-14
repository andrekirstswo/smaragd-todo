using Api.Database;
using Api.Infrastructure;
using Core;
using Core.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

namespace Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        });
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
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddCookie()
            .AddJwtBearer(options =>
            {
                var section = builder.Configuration.GetSection("Authentication:Google");
                options.Authority = section[nameof(GoogleAuthenticationOptions.Authority)];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = section[nameof(GoogleAuthenticationOptions.ValidIssuer)],
                    ValidateAudience = true,
                    ValidAudiences = new [] { section[nameof(GoogleAuthenticationOptions.ClientId)] },
                    ValidateLifetime = true
                };
            });
        builder.Services.AddAuthorization();

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
                    .WithOrigins("http://localhost:4200")
                    .AllowCredentials();
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

        app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

        app.Run();
    }
}
