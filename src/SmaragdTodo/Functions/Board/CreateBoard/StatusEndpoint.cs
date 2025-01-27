using System.Net;
using Functions.Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;

namespace Functions.Board.CreateBoard
{
    public class StatusEndpoint
    {
        private readonly CosmosClient _cosmosClient;

        public StatusEndpoint(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [Function("StatusEndpoint")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "request/board/status/{requestId}")] HttpRequest httpRequest,
            string requestId)
        {
            var container = _cosmosClient.GetContainer(Constants.DatabaseName, ContainerNames.Boards);

            var response = await container.ReadItemAsync<BoardEntity>(requestId, new PartitionKey("/Id"));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new StatusCodeResult((int)HttpStatusCode.Found);
            }

            return new NotFoundObjectResult(new { id = requestId });
        }
    }
}
