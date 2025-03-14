using Api.Infrastructure;

namespace Api.Features.Board.GetBoardNameById;

public class GetBoardNameByIdQuery : Query<string>
{
    public GetBoardNameByIdQuery(string boardId)
    {
        BoardId = boardId;
    }

    public string BoardId { get; }
}