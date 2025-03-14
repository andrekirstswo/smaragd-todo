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

    public Task<IActionResult> Get(string boardId, string taskId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> Create(string boardId, CreateTaskDto model, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}