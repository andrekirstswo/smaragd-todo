using System.Net;
using Api.Features.Board.CreateBoard;
using Api.Features.Board.GetBoardById;
using Api.Features.Board.GetBoardNameById;
using Api.Features.Board.GetBoards;
using Core;
using Core.Models;
using Core.Models.ValueObjects;
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
        var result = await _mediator.Send(new CreateBoardCommand
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
        var result = await _mediator.Send(new GetBoardsQuery());
        var boards = result.Value;

        return boards != null && boards.Any() ? Ok(result.Value) : NoContent();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetBoardByIdDto), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBoardByIdQuery(BoardId.From(id)), cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id}/name")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetNameById(string id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBoardNameByIdQuery(BoardId.From(id)), cancellationToken);

        return HandleResult(result);
    }

    private IActionResult HandleResult<TValue, TError>(Result<TValue, TError> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}