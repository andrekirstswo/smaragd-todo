using Api.Extensions;
using Api.Infrastructure;
using Core;
using Core.Models;
using ErrorHandling;
using Microsoft.Azure.CosmosRepository;

namespace Api.Features.Board.GetBoards;

public class GetBoardsQueryHandler : QueryHandler<GetBoardsQuery, List<GetBoardsDto>>
{
    private readonly IRepository<Core.Database.Models.Board> _boardRepository;
    private readonly HttpContext _httpContext;

    public GetBoardsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        IRepository<Core.Database.Models.Board> boardRepository)
    {
        _boardRepository = boardRepository;
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
    }

    public override async Task<Result<List<GetBoardsDto>, Error>> Handle(GetBoardsQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContext.User.GetUserId();

        // TODO Refactor to query with selected properties
        var result = await _boardRepository.GetAsync(p =>
                p.Owner == userId ||
                (p.Accesses != null && p.Accesses.Any(a => a.UserId == userId)),
            cancellationToken);
        
        return result
            .Select(b => new GetBoardsDto
            {
                BoardId = b.BoardId,
                Name = b.Name
            })
            .ToList();
    }
}