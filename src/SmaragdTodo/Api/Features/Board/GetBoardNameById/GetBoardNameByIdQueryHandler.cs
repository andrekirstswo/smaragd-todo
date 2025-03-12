using Api.Infrastructure;
using Core;
using Microsoft.Azure.CosmosRepository;

namespace Api.Features.Board.GetBoardNameById;

public class GetBoardNameByIdQueryHandler : QueryHandler<GetBoardNameByIdQuery, string>
{
    private readonly IRepository<Core.Database.Models.Board> _boardRepository;

    public GetBoardNameByIdQueryHandler(IRepository<Core.Database.Models.Board> boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public override async Task<Result<string, Error>> Handle(GetBoardNameByIdQuery request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetAsync(request.BoardId.Value, cancellationToken: cancellationToken);

        if (string.IsNullOrEmpty(board.Name))
        {
            return KnownErrors.Board.HasNoName(request.BoardId);
        }

        return board.Name;
    }
}