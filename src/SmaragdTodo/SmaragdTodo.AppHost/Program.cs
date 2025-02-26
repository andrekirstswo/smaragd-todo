var builder = DistributedApplication.CreateBuilder(args);

var cosmosDb = builder.ExecutionContext.IsPublishMode
    ? builder
        .AddAzureCosmosDB("cosmosdb")
        .AddDatabase("SmaragdTodo")
    : builder.AddConnectionString("cosmosdb");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("messaging")
    : builder.AddConnectionString("messaging");

var functions = builder
    .AddAzureFunctionsProject<Projects.Functions>("functions")
    .WithExternalHttpEndpoints()
    .WithReference(serviceBus)
    .WithReference(cosmosDb);

var api = builder.AddProject<Projects.Api>("apiservice")
    .WithReference(serviceBus)
    .WithReference(cosmosDb);

builder.AddProject<Projects.WebUI>("webui")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
