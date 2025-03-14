using System.Net;
using Api.Features.Task.CreateTask;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("/api/board/{boardId}/status/{taskId}")]
    public Task<IActionResult> Status(string boardId, string taskId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [HttpGet("/api/board/{boardId}/task/{taskId}")]
    public Task<IActionResult> Get(string boardId, string taskId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [HttpPost("/api/board/{boardId}/task")]
    [ProducesResponseType(typeof(CreateTaskResponseDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Create(string boardId, [FromBody] CreateTaskDto model, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new CreateTaskCommand(boardId, model), cancellationToken);

        return result.IsSuccess
            ? Accepted(result.Value!.StatusUrl, new { requestStatusUrl = result.Value!.StatusUrl })
            : BadRequest(result.Error);
    }
}