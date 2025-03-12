using Api.Infrastructure;
using Core.Models;
using Core.Models.ValueObjects;

namespace Api.Features.Board.GetBoardById;

public class GetBoardByIdQuery : Query<GetBoardByIdDto>
{
    public BoardId BoardId { get; }

    public GetBoardByIdQuery(BoardId boardId)
    {
        BoardId = boardId;
    }
}