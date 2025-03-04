using System.Text.Json;
using Core;
using Core.Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.AddServiceDefaults();

builder.AddAzureCosmosClient(ConnectionNames.CosmosDb, configureClientOptions: options =>
{
    options.UseSystemTextJsonSerializerWithOptions = JsonSerializerOptions.Web;
});

builder.AddAzureBlobClient(ConnectionNames.Blobs);
builder.AddAzureServiceBusClient(ConnectionNames.Messaging);

builder.Services
    .AddSignalR()
    .AddNamedAzureSignalR("signalr");

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
