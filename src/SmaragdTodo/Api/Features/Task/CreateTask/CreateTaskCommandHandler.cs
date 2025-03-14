using Api.Infrastructure;
using Core;
using Core.Models;
using ErrorHandling;
using Events;

namespace Api.Features.Task.CreateTask;

public class CreateTaskCommandHandler : CommandHandler<CreateTaskCommand, CreateTaskResponseDto>
{
    private readonly IMessaging _messaging;
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _httpContext;

    public CreateTaskCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMessaging messaging,
        LinkGenerator linkGenerator)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
        _messaging = messaging;
        _linkGenerator = linkGenerator;
    }

    public override async Task<Result<CreateTaskResponseDto, Error>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskId = Guid.CreateVersion7().ToString();
        var boardId = request.BoardId;
        var requestStatusUrl = _linkGenerator.GetUriByAction(_httpContext, "Status", "Task", new { boardId, taskId });

        ArgumentException.ThrowIfNullOrEmpty(requestStatusUrl);

        var @event = new TaskCreatedEvent(request.BoardId, taskId, request.Model.Title, request.Model.AssignedTo);

        var applicationProperties = new Dictionary<string, object>
        {
            { Constants.Request.RequestId, Guid.CreateVersion7().ToString() },
            { Constants.Request.RequestStatusUrl, requestStatusUrl }
        };

        await _messaging.SendEventAsync(@event, applicationProperties, cancellationToken);

        return new CreateTaskResponseDto
        {
            StatusUrl = requestStatusUrl
        };
    }
}