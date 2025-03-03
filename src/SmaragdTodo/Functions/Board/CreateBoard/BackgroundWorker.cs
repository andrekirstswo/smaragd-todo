using System.Net;
using Azure.Messaging.ServiceBus;
using Core;
using Core.Database.Models;
using Core.Infrastructure;
using Events;
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
        [ServiceBusTrigger(QueueNames.Board.Created, Connection = ConnectionNames.Messaging)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var requestId = message.ApplicationProperties[Constants.Request.RequestId].ToString()!;
        var createBoardRequest = message.Body.ToObjectFromJson<BoardCreatedEvent>();
        
        ArgumentNullException.ThrowIfNull(createBoardRequest);

        var database = _cosmosClient.GetDatabase(Constants.DatabaseName);
        var boardsContainer = database.GetContainer(ContainerNames.Boards);

        var userId = createBoardRequest.Owner;
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var board = new Core.Database.Models.Board
        {
            Id = createBoardRequest.BoardId,
            Name = createBoardRequest.Name,
            CreatedAt = _dateTimeProvider.UtcNow,
            Owner = userId,
            Accesses = new List<BoardUserAccess>
            {
                new BoardUserAccess
                {
                    UserId = userId,
                    Role = BoardUserAccessRoles.Admin
                }
            }
        };

        var response = await boardsContainer.UpsertItemAsync(board, new PartitionKey(board.Id));

        var success = response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;

        if (success)
        {
            await messageActions.CompleteMessageAsync(message);
            _logger.LogInformation("Message {requestId} successfully completed", requestId);
        }
        else
        {
            if (message.DeliveryCount >= 10)
            {
                await messageActions.DeadLetterMessageAsync(message, deadLetterReason: "Delivery count reached");
                _logger.LogWarning("Message moved to Dead-Letter-Queue");
            }
            else
            {
                throw new Exception("Could not create item in db");
            }
        }
    }
}
