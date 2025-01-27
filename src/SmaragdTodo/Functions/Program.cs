using Core;
using Functions.Infrastructure;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.AddServiceDefaults();

builder.AddAzureCosmosClient(ConnectionNames.CosmosDb);

builder.AddAzureBlobClient(ConnectionNames.Blobs);
builder.AddAzureServiceBusClient(ConnectionNames.Messaging);

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
