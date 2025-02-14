using System.Net;
using Azure.Messaging.ServiceBus;
using Core;
using Core.Database.Models;
using Core.Infrastructure;
using Events;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using User = Core.Database.Models.User;

namespace Functions.Board.CreateBoard;

public class BackgroundWorker
{
    private readonly CosmosClient _cosmosClient;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<BackgroundWorker> _logger;
    private readonly CosmosTransactionManager _cosmosTransactionManager;

    public BackgroundWorker(
        CosmosClient cosmosClient,
        IDateTimeProvider dateTimeProvider,
        ILogger<BackgroundWorker> logger,
        CosmosTransactionManager cosmosTransactionManager)
    {
        _cosmosClient = cosmosClient;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _cosmosTransactionManager = cosmosTransactionManager;
    }

    [Function(nameof(BackgroundWorker))]
    public async Task Run(
        [ServiceBusTrigger(QueueNames.Board.Created, Connection = ConnectionNames.Messaging)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var requestId = message.ApplicationProperties[Constants.Request.RequestId].ToString()!;
        var database = _cosmosClient.GetDatabase(Constants.DatabaseName);
        var boardsContainer = database.GetContainer(ContainerNames.Boards);
        var usersContainer = database.GetContainer(ContainerNames.Users);
        var boardUserAccessesContainer = database.GetContainer(ContainerNames.BoardUserAccesses);
        var createBoardRequest = message.Body.ToObjectFromJson<BoardCreatedEvent>();

        if (createBoardRequest is null)
        {
            _logger.LogError("{request} is null", createBoardRequest);
            ArgumentNullException.ThrowIfNull(createBoardRequest);
        }

        var boardId = createBoardRequest.BoardId;

        var userId = await GetUserIdByEmail(usersContainer, createBoardRequest.Owner);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        // TODO Refactor or more analysis to handle better with id and partition key
        var boardEntity = new Core.Database.Models.Board
        {
            id = boardId,
            BoardId = boardId,
            Name = createBoardRequest.Name,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        var boardUserAccessEntityId = Guid.NewGuid().ToString();
        var boardUserAccessEntity = new BoardUserAccess
        {
            id = boardUserAccessEntityId,
            BoardUserAccessId = boardUserAccessEntityId,
            CreatedAt = _dateTimeProvider.UtcNow,
            Role = BoardUserAccessRole.Admin,
            BoardId = boardId,
            UserId = userId
        };

        _cosmosTransactionManager.AddOperation(boardsContainer, boardEntity, new PartitionKey(boardEntity.BoardId));
        _cosmosTransactionManager.AddOperation(boardUserAccessesContainer, boardUserAccessEntity, new PartitionKey(boardUserAccessEntity.BoardId));

        var success = await _cosmosTransactionManager.ExecuteTransactionAsync();

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

        //var responseCreateBoard = await boardsContainer.CreateItemAsync(boardEntity, new PartitionKey(boardEntity.Id));

        //if (responseCreateBoard.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created)
        //{
        //    await messageActions.CompleteMessageAsync(message);
        //    _logger.LogInformation("Message {requestId} successfully completed", requestId);
        //}
        //else
        //{
        //    if (message.DeliveryCount >= 10)
        //    {
        //        await messageActions.DeadLetterMessageAsync(message, deadLetterReason: "Delivery count reached");
        //        _logger.LogWarning("Message moved to Dead-Letter-Queue");
        //    }
        //    else
        //    {
        //        throw new Exception($"Could not create item in db. HttpStatusCode: {responseCreateBoard.StatusCode}");
        //    }
        //}
    }

    private async Task<string?> GetUserIdByEmail(Container container, string email, CancellationToken cancellationToken = default)
    {
        return await container.Get<string, User>(
            "SELECT * FROM c WHERE c.Email = @email",
            item => item.UserId,
            new Dictionary<string, object>
            {
                { "@email", email.ToLower() }
            },
            cancellationToken);

        //var query = new QueryDefinition("SELECT * FROM c WHERE c.Email = @email")
        //    .WithParameter("@email", email.ToLower());

        //using var resultSet = container.GetItemQueryIterator<UserEntity>(query);

        //if (!resultSet.HasMoreResults) return null;

        //foreach (var userEntity in (await resultSet.ReadNextAsync(cancellationToken)))
        //{
        //    return userEntity.UserId;
        //}

        //return null;
    }
}

public static class ContainerExtensions
{
    public static async Task<TResult?> Get<TResult, TEntity>(
        this Container container,
        string sql,
        Func<TEntity, TResult> select,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition(sql);

        if (parameters is not null)
        {
            foreach (var parameter in parameters)
            {
                query = query
                    .WithParameter(parameter.Key, parameter.Value);
            }
        }

        using var resultSet = container.GetItemQueryIterator<TEntity>(query);

        if (!resultSet.HasMoreResults)
        {
            return default;
        }

        foreach (var item in await resultSet.ReadNextAsync(cancellationToken))
        {
            return select(item);
        }

        return default;
    }
}

public class CosmosTransactionManager
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<CosmosTransactionManager> _logger;
    private readonly List<(Container Container, string Id, PartitionKey PartitionKey)> _rollbacks = new List<(Container Container, string Id, PartitionKey PartitionKey)>();
    private readonly List<(Container Container, dynamic Item, PartitionKey PartitionKey)> _operations = new List<(Container Container, dynamic Item, PartitionKey PartitionKey)>();

    public CosmosTransactionManager(
        CosmosClient cosmosClient,
        ILogger<CosmosTransactionManager> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    public void AddOperation(Container container, dynamic item, PartitionKey partitionKey)
    {
        _operations.Add((container, item, partitionKey));
    }

    public async Task<bool> ExecuteTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var operation in _operations)
            {
                var response = await operation.Container.UpsertItemAsync(operation.Item, operation.PartitionKey, cancellationToken: cancellationToken);

                if (response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK)
                {
                    _rollbacks.Add((operation.Container, operation.Item.id, operation.PartitionKey));
                }
                else
                {
                    throw new Exception($"Fehlgeschlagen beim Speichern von {operation.Item.id}");
                }
            }

            _rollbacks.Clear();

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await RollbackAsync(cancellationToken);
            return false;
        }
    }

    private async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        foreach (var rollback in _rollbacks)
        {
            try
            {
                var response = await rollback.Container.DeleteItemAsync<dynamic>(rollback.Id, rollback.PartitionKey, cancellationToken: cancellationToken);
                if (response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent)
                {
                    _logger.LogInformation("Rollback successfully: {Id}", rollback.Id);
                }
                else
                {
                    _logger.LogInformation("Rollback with StatusCode {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Rollback failed for {Id}: {Message}", rollback.Id, e.Message);
            }

            _rollbacks.Clear();
        }
    }
}