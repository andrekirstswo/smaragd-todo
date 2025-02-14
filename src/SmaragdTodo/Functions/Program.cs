using Core;
using Core.Infrastructure;
using Functions.Board.CreateBoard;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<CosmosTransactionManager>();

builder.AddServiceDefaults();

builder.AddAzureCosmosClient(ConnectionNames.CosmosDb);

builder.AddAzureBlobClient(ConnectionNames.Blobs);
builder.AddAzureServiceBusClient(ConnectionNames.Messaging);

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
