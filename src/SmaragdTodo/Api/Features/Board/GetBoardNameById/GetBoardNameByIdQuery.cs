using Api.Infrastructure;
using Core.Models.ValueObjects;

namespace Api.Features.Board.GetBoardNameById;

public class GetBoardNameByIdQuery : Query<string>
{
    public GetBoardNameByIdQuery(BoardId boardId)
    {
        BoardId = boardId;
    }

    public BoardId BoardId { get; }
}