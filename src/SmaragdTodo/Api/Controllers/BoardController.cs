using Api.Features.Board.CreateBoard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BoardController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBoardModel model)
    {
        var result = await _mediator.Send(new CreateBoardRequest
        {
            Name = model.Name
        });

        return result.IsSuccess
            ? Accepted(result.Value!.Url, new { requestStatusUrl = result.Value!.Url })
            : BadRequest(result.Error);
    }

    [HttpGet("status/{id}")]
    public Task<IActionResult> Status(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}