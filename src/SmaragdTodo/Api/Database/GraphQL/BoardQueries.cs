using Core.Extensions;
using Core.Models;

namespace Api.Database.GraphQL;

public class BoardQueries
{
    public async Task<IEnumerable<Board>> GetBoards(
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] BoardRepository boardRepository,
        CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        ArgumentNullException.ThrowIfNull(httpContext);

        var userId = httpContext.User.GetUserId();

        var result = await boardRepository.GetAllAsync(userId, cancellationToken);

        return result.Select(Board.Map);
    }

    public async Task<Board?> GetBoard(
        [Service] BoardRepository boardRepository,
        string boardId,
        CancellationToken cancellationToken = default)
    {
        var result = await boardRepository.GetByIdAsync(boardId, cancellationToken);

        return result is null ? null : Board.Map(result);
    }
}