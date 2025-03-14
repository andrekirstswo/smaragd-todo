using Api.Infrastructure;
using Core.Models;

namespace Api.Features.Board.GetBoardById;

public class GetBoardByIdQuery : Query<GetBoardByIdDto>
{
    public string BoardId { get; }

    public GetBoardByIdQuery(string boardId)
    {
        BoardId = boardId;
    }
}