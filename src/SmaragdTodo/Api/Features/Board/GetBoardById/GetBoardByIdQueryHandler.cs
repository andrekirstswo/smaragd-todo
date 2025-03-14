using Api.Database;
using Api.Infrastructure;
using Core.Models;
using Core;
using ErrorHandling;

namespace Api.Features.Board.GetBoardById;

public class GetBoardByIdQueryHandler : QueryHandler<GetBoardByIdQuery, GetBoardByIdDto>
{
    private readonly BoardRepository _boardRepository;

    public GetBoardByIdQueryHandler(BoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public override async Task<Result<GetBoardByIdDto, Error>> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(request.BoardId, cancellationToken: cancellationToken);

        if (board is null)
        {
            return KnownErrors.Board.NotFoundById(request.BoardId);
        }

        return new GetBoardByIdDto
        {
            Name = board.Name,
            Owner = board.Owner,
            Accesses = board.Accesses?.Select(a => new BoardUserAccess(a.UserId, a.Role)) ?? new List<BoardUserAccess>(),
            BoardId = board.BoardId,
            Sections = board.Sections?.Select(s => new BoardSection
            {
                BoardSectionId = s.BoardSectionId,
                Name = s.Name,
                Order = s.Order
            }) ?? new List<BoardSection>()
        };
    }
}