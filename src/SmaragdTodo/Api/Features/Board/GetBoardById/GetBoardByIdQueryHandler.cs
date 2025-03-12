using Api.Infrastructure;
using Core.Models;
using Core;
using Microsoft.Azure.CosmosRepository;

namespace Api.Features.Board.GetBoardById;

public class GetBoardByIdQueryHandler : QueryHandler<GetBoardByIdQuery, GetBoardByIdDto>
{
    private readonly IRepository<Core.Database.Models.Board> _boardRepository;

    public GetBoardByIdQueryHandler(IRepository<Core.Database.Models.Board> boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public override async Task<Result<GetBoardByIdDto, Error>> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.TryGetAsync(request.BoardId.Value, cancellationToken: cancellationToken);

        if (board is null)
        {
            return KnownErrors.Board.NotFoundById(request.BoardId);
        }

        return new GetBoardByIdDto
        {
            Name = board.Name,
            Owner = board.Owner,
            Accesses = board.Accesses?.Select(a => new BoardUserAccess(a.UserId, a.Role)) ?? new List<BoardUserAccess>(),
            BoardId = board.Id,
            Sections = board.Sections?.Select(s => new BoardSection(s.Id, s.Name, s.Order)) ?? new List<BoardSection>()
        };
    }
}