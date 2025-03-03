using System.Net;
using Api.Features.Board.CreateBoard;
using Api.Features.Board.GetBoards;
using Core.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = Core.Constants.Token.Scheme)]
public class BoardController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateBoardResponseDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Create([FromBody] CreateBoardDto model)
    {
        var result = await _mediator.Send(new CreateBoardRequest
        {
            Name = model.Name
        });

        return result.IsSuccess
            ? Accepted(result.Value!.StatusUrl, new { requestStatusUrl = result.Value!.StatusUrl })
            : BadRequest(result.Error);
    }

    [HttpGet("status/{id}")]
    public Task<IActionResult> Status(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetBoardsDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetBoardsRequest());
        var boards = result.Value;

        return boards != null && boards.Any() ? Ok(result.Value) : NoContent();
    }
}