using System.Security.Claims;
using Api.Infrastructure;
using Core;
using Events;
using MediatR;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardRequestHandler : IRequestHandler<CreateBoardRequest, Result<CreateBoardRequestResult, Error>>
{
    private readonly IMessaging _messaging;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _httpContext;

    public CreateBoardRequestHandler(
        IHttpContextAccessor httpContextAccessor,
        IMessaging messaging,
        LinkGenerator linkGenerator)
    {
        _messaging = messaging;
        _linkGenerator = linkGenerator;
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<Result<CreateBoardRequestResult, Error>> Handle(CreateBoardRequest request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        var requestStatusUrl = _linkGenerator.GetUriByAction(_httpContext, "Status", "Board", new { id = requestId });

        ArgumentException.ThrowIfNullOrEmpty(requestStatusUrl);

        if (!_httpContext.User.TryGetClaimByName(ClaimTypes.Email, out var owner))
        {
            return new Error("CLAIM_NOT_FOUND", $"Claim {ClaimTypes.Email} not found");
        }

        var @event = new BoardCreatedEvent(requestId, request.Name, owner);

        var applicationProperties = new Dictionary<string, object>
        {
            { Constants.Request.RequestId, requestId },
            { Constants.Request.RequestStatusUrl, requestStatusUrl }
        };

        await _messaging.PrepareAndSendMessageAsync(@event, applicationProperties, cancellationToken);

        return new CreateBoardRequestResult
        {
            Url = requestStatusUrl
        };
    }
}