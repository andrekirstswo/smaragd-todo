using System.Net;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Core;
using Core.Database.Models;
using Core.Infrastructure;
using Events;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Notifications;

namespace Functions;

public class TaskCreatedBackgroundWorker
{
    private readonly CosmosClient _cosmosClient;
    private readonly ILogger<TaskCreatedBackgroundWorker> _logger;

    public TaskCreatedBackgroundWorker(
        CosmosClient cosmosClient,
        ILogger<TaskCreatedBackgroundWorker> logger)
    {
        _cosmosClient = cosmosClient;
        _logger = logger;
    }

    [Function(nameof(TaskCreatedBackgroundWorker))]
    [ServiceBusOutput(QueueNames.Task.CreatedNotification, Connection = ConnectionNames.Messaging)]
    public async Task<TaskCreatedNotification> Run(
        [ServiceBusTrigger(QueueNames.Task.Created, Connection = ConnectionNames.Messaging)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var requestId = message.ApplicationProperties[Constants.Request.RequestId].ToString()!;
        var @event = await JsonSerializer.DeserializeAsync<TaskCreatedEvent>(message.Body.ToStream(), DefaultJsonSerializerOptions.Value);
        
        ArgumentNullException.ThrowIfNull(@event);

        var tasksContainer = _cosmosClient.GetContainer(Constants.DatabaseName, ContainerNames.Tasks);

        var taskItem = new TaskItem
        {
            AssignedTo = @event.AssignedTo,
            BoardId = @event.BoardId,
            TaskId = @event.TaskId,
            Title = @event.Title
        };

        var response = await tasksContainer.UpsertItemAsync(taskItem, new PartitionKey(taskItem.BoardId));

        var success = response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created;

        if (success)
        {
            await messageActions.CompleteMessageAsync(message);

            _logger.LogInformation("Message {requestId} successfully completed", requestId);
            
            return new TaskCreatedNotification
            {
                BoardId = @event.BoardId,
                TaskId = @event.TaskId,
                Title = @event.Title,
                AssignedTo = @event.AssignedTo
            };
        }

        if (message.DeliveryCount >= 10)
        {
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: "Delivery count reached");
            _logger.LogWarning("Message moved to Dead-Letter-Queue");
        }
        
        throw new Exception("Could not create item in db");
    }
}
