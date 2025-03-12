using Api.Extensions;
using Api.Infrastructure;
using Core;
using Core.Infrastructure;
using Core.Models;
using Events;
using BoardSection = Core.Models.BoardSection;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardCommandHandler : CommandHandler<CreateBoardCommand, CreateBoardResponseDto>
{
    private readonly IMessaging _messaging;
    private readonly LinkGenerator _linkGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly HttpContext _httpContext;

    public CreateBoardCommandHandler(
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

    public override async Task<Result<CreateBoardResponseDto, Error>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var requestId = Guid.CreateVersion7(_dateTimeProvider.UtcNow).ToString();
        var requestStatusUrl = _linkGenerator.GetUriByAction(_httpContext, "Status", "Board", new { id = requestId });

        ArgumentException.ThrowIfNullOrEmpty(requestStatusUrl);

        var owner = _httpContext.User.UserId();

        var sections = new List<BoardSection>
        {
            new(Guid.CreateVersion7(_dateTimeProvider.UtcNow).ToString(), "New", 1),
            new(Guid.CreateVersion7(_dateTimeProvider.UtcNow).ToString(), "In progress", 2),
            new(Guid.CreateVersion7(_dateTimeProvider.UtcNow).ToString(), "Done", 3)
        };

        var @event = new BoardCreatedEvent(
            requestId,
            request.Name,
            owner,
            _dateTimeProvider.UtcNow,
            sections);

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