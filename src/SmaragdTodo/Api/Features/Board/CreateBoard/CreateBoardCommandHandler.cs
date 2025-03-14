using Api.Extensions;
using Api.Infrastructure;
using Core;
using Core.Models;
using ErrorHandling;
using Events;
using BoardSection = Core.Models.BoardSection;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardCommandHandler : CommandHandler<CreateBoardCommand, CreateBoardResponseDto>
{
    private readonly IMessaging _messaging;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _httpContext;

    public CreateBoardCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMessaging messaging,
        LinkGenerator linkGenerator)
    {
        _messaging = messaging;
        _linkGenerator = linkGenerator;
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
    }

    public override async Task<Result<CreateBoardResponseDto, Error>> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var boardId = Guid.CreateVersion7().ToString();
        var requestStatusUrl = _linkGenerator.GetUriByAction(_httpContext, "Status", "Board", new { id = boardId });

        ArgumentException.ThrowIfNullOrEmpty(requestStatusUrl);

        var owner = _httpContext.User.GetUserId();

        var sections = new List<BoardSection>
        {
            new BoardSection
            {
                BoardSectionId = Guid.CreateVersion7().ToString(),
                Name = "New",
                Order = 1
            },
            new BoardSection
            {
                BoardSectionId = Guid.CreateVersion7().ToString(),
                Name = "In progress",
                Order = 2
            },
            new BoardSection
            {
                BoardSectionId = Guid.CreateVersion7().ToString(),
                Name = "Done",
                Order = 3
            }
        };

        var @event = new BoardCreatedEvent(boardId, request.Name, owner, sections);

        var applicationProperties = new Dictionary<string, object>
        {
            { Constants.Request.RequestId, boardId },
            { Constants.Request.RequestStatusUrl, requestStatusUrl }
        };

        await _messaging.SendEventAsync(@event, applicationProperties, cancellationToken);

        return new CreateBoardResponseDto
        {
            StatusUrl = requestStatusUrl
        };
    }
}