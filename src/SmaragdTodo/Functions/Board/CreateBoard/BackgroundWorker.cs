using System.Net;
using Azure.Messaging.ServiceBus;
using Core;
using Functions.Database.Model;
using Functions.Infrastructure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions.Board.CreateBoard;

public class BackgroundWorker
{
    private readonly CosmosClient _cosmosClient;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<BackgroundWorker> _logger;

    public BackgroundWorker(
        CosmosClient cosmosClient,
        IDateTimeProvider dateTimeProvider,
        ILogger<BackgroundWorker> logger)
    {
        _cosmosClient = cosmosClient;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    [Function(nameof(BackgroundWorker))]
    public async Task Run(
        [ServiceBusTrigger(QueueNames.Board.Create, Connection = ConnectionNames.Messaging)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var requestId = message.ApplicationProperties[Constants.Request.RequestId].ToString()!;

        var container = _cosmosClient.GetContainer(Constants.DatabaseName, ContainerNames.Boards);
        var createBoardRequest = message.Body.ToObjectFromJson<CreateBoardRequest>();

        if (createBoardRequest is null)
        {
            _logger.LogError("{request} is null", createBoardRequest);
            throw new ArgumentNullException(nameof(createBoardRequest));
        }

        // TODO Refactor or more analysis to handle better with id and partition key
        var item = new BoardEntity
        {
            id = requestId,
            Id = requestId,
            Name = createBoardRequest.Name,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        var response = await container.CreateItemAsync(item, new PartitionKey(item.Id));

        if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created)
        {
            await messageActions.CompleteMessageAsync(message);
            _logger.LogInformation("Message {requestId} successfully completed", requestId);
        }
        else
        {
            if (message.DeliveryCount >= 3)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: "Delivery count reached");
                _logger.LogWarning("Message moved to Dead-Letter-Queue");
            }
            else
            {
                throw new Exception($"Could not create item in db. HttpStatusCode: {response.StatusCode}");
            }
        }
    }
}