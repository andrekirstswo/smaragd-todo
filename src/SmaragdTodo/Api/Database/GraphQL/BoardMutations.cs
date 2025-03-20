using Api.Features.Board.CreateBoard;
using Core.Models;
using MediatR;

namespace Api.Database.GraphQL;

public class BoardMutations
{
    public async Task<CreateBoardResponseDto> CreateBoard(
        [Service] IMediator mediator,
        CreateBoardDto dto,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateBoardCommand
        {
            Name = dto.Name
        };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return result.Value!;
        }

        throw new Exception(result.Error!.Message);
    }
}