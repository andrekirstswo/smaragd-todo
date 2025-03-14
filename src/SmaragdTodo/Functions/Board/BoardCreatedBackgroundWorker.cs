using System.Net;
using Azure.Messaging.ServiceBus;
using Core;
using Core.Database.Models;
using Events;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications;

namespace Functions.Board;

public class BoardCreatedBackgroundWorker
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<BoardCreatedBackgroundWorker> _logger;

    public BoardCreatedBackgroundWorker(
        CosmosClient cosmosClient,
        ILogger<BoardCreatedBackgroundWorker> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    [Function(nameof(BoardCreatedBackgroundWorker))]
    [ServiceBusOutput(QueueNames.Board.CreatedNotification, Connection = ConnectionNames.Messaging)]
    public async Task<BoardCreatedNotification> Run(
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
            Owner = userId,
            Accesses = new List<BoardUserAccess>
            {
                new BoardUserAccess
                {
                    UserId = userId,
                    Role = BoardUserAccessRoles.Admin
                }
            },
            Sections = createBoardRequest.Sections?.Select(s => new BoardSection
            {
                Id = s.Id,
                Name = s.Name,
                Order = s.Order
            }).ToList() ?? new List<BoardSection>()
        };

        var response = await boardsContainer.UpsertItemAsync(board, new PartitionKey(board.Id));

        var success = response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;

        if (success)
        {
            await messageActions.CompleteMessageAsync(message);

            _logger.LogInformation("Message {requestId} successfully completed", requestId);
            
            return new BoardCreatedNotification(
                createBoardRequest.BoardId,
                createBoardRequest.Name,
                createBoardRequest.Owner,
                createBoardRequest.Sections ?? new List<Core.Models.BoardSection>());
        }

        if (message.DeliveryCount >= 10)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: "Delivery count reached");
            _logger.LogWarning("Message moved to Dead-Letter-Queue");
        }
        
        throw new Exception("Could not create item in db");
    }
}
