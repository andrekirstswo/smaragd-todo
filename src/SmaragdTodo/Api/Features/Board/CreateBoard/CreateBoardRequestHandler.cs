using Api.Extensions;
using Api.Infrastructure;
using Core;
using Core.Infrastructure;
using Core.Models;
using Events;
using MediatR;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardRequestHandler : IRequestHandler<CreateBoardRequest, Result<CreateBoardResponseDto, Error>>
{
    private readonly IMessaging _messaging;
    private readonly LinkGenerator _linkGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly HttpContext _httpContext;

    public CreateBoardRequestHandler(
        IHttpContextAccessor httpContextAccessor,
        IMessaging messaging,
        LinkGenerator linkGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _messaging = messaging;
        _linkGenerator = linkGenerator;
        _dateTimeProvider = dateTimeProvider;
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<Result<CreateBoardResponseDto, Error>> Handle(CreateBoardRequest request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        var requestStatusUrl = _linkGenerator.GetUriByAction(_httpContext, "Status", "Board", new { id = requestId });

        ArgumentException.ThrowIfNullOrEmpty(requestStatusUrl);

        var owner = _httpContext.User.UserId();

        var @event = new BoardCreatedEvent(requestId, request.Name, owner, _dateTimeProvider.UtcNow);

        var applicationProperties = new Dictionary<string, object>
        {
            { Constants.Request.RequestId, requestId },
            { Constants.Request.RequestStatusUrl, requestStatusUrl }
        };

        await _messaging.PrepareAndSendMessageAsync(@event, applicationProperties, cancellationToken);

        return new CreateBoardResponseDto
        {
            StatusUrl = requestStatusUrl
        };
    }
}