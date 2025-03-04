using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Functions;

public class Negotiate
{
    private readonly ILogger<Negotiate> _logger;

    public Negotiate(ILogger<Negotiate> logger)
    {
        _logger = logger;
    }

    [Function("negotiate")]
    public string Run(
        [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
        [SignalRConnectionInfoInput(HubName = "notifications")] string connectionInfo)
    {
        _logger.LogInformation(connectionInfo);
        return connectionInfo;
    }
}