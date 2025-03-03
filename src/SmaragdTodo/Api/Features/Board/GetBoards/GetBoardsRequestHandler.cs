using Api.Database;
using Api.Extensions;
using Core;
using Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Board.GetBoards;

public class GetBoardsRequestHandler : IRequestHandler<GetBoardsRequest, Result<List<GetBoardsDto>, Error>>
{
    private readonly SmaragdTodoContext _dbContext;
    private readonly HttpContext _httpContext;

    public GetBoardsRequestHandler(
        IHttpContextAccessor httpContextAccessor,
        SmaragdTodoContext dbContext)
    {
        _dbContext = dbContext;
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);
        _httpContext = httpContextAccessor.HttpContext;
    }

    public async Task<Result<List<GetBoardsDto>, Error>> Handle(GetBoardsRequest request, CancellationToken cancellationToken)
    {
        var userId = _httpContext.User.UserId();

        return await _dbContext.Boards
            .Where(b => b.Owner == userId ||
                        b.Accesses.Any(a => a.UserId == userId))
            .Select(b => new GetBoardsDto
            {
                Id = b.Id,
                Name = b.Name
            })
            .ToListAsync(cancellationToken);
    }
}