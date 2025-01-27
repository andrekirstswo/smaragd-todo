using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Functions.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Functions.Board.CreateBoard
{
    public class EndpointFunction
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EndpointFunction(
            ServiceBusClient serviceBusClient,
            IDateTimeProvider dateTimeProvider)
        {
            _serviceBusClient = serviceBusClient;
            _dateTimeProvider = dateTimeProvider;
        }

        [Function($"{nameof(EndpointFunction)}")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "board")] HttpRequest httpRequest,
            [Microsoft.Azure.Functions.Worker.Http.FromBody] CreateBoardRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return new BadRequestResult();
            }

            var requestId = Guid.NewGuid().ToString();
            var requestStatusUrl = Urls.BoardRequestStatus(requestId);
            var payload = JsonSerializer.Serialize(request);
            var message = new ServiceBusMessage(payload);
            message.ApplicationProperties.Add(Constants.Request.RequestId, requestId);
            message.ApplicationProperties.Add(Constants.Request.RequestSubmittedAt, _dateTimeProvider.UtcNow);
            message.ApplicationProperties.Add(Constants.Request.RequestStatusUrl, requestStatusUrl);
            
            var sender = _serviceBusClient.CreateSender(QueueNames.Board.Create);
            await sender.SendMessageAsync(message);

            return new AcceptedResult(requestStatusUrl, $"Request Accepted for Processing{Environment.NewLine}ProxyStatus: {requestStatusUrl}");
        }
    }
}
