var builder = DistributedApplication.CreateBuilder(args);

var apicache = builder
    .AddAzureRedis("apicache")
    .WithAccessKeyAuthentication()
    .RunAsContainer();

var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR("signalr")
    : builder.AddConnectionString("signalr");

var cosmosDb = builder.ExecutionContext.IsPublishMode
    ? builder
        .AddAzureCosmosDB("cosmosdb")
        .AddCosmosDatabase("SmaragdTodo")
    : builder.AddConnectionString("cosmosdb");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("messaging")
    : builder.AddConnectionString("messaging");

builder
    .AddAzureFunctionsProject<Projects.Functions>("functions")
    .WithExternalHttpEndpoints()
    .WithReference(serviceBus)
    .WithReference(cosmosDb)
    .WithReference(signalr);

var api = builder.AddProject<Projects.Api>("apiservice")
    .WithReference(serviceBus)
    .WithReference(cosmosDb)
    .WithReference(apicache)
    .WithReference(signalr);

builder.AddProject<Projects.WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api)
    .WithReference(signalr);

builder.Build().Run();
