using Core.Database.Models;
using Microsoft.Azure.CosmosRepository;

namespace Api.Database;

public class BoardRepository
{
    private readonly IRepository<Board> _repository;

    public BoardRepository(IRepository<Board> repository)
    {
        _repository = repository;
    }

    public async Task<Board?> GetByIdAsync(string boardId, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(p => p.BoardId == boardId, cancellationToken: cancellationToken);
        return result.SingleOrDefault();
    }

    public async Task<bool> IsUserInRolesAsync(string boardId, string userId, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(b =>
            b.BoardId == boardId &&
            b.Accesses != null &&
            b.Accesses.Any(a =>
                a.UserId == userId &&
                roles.Contains(a.Role)), cancellationToken);
    }

    public async Task<bool> IsUserOwnerOfBoardByIdAsync(string boardId, string userId, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(p => p.BoardId == boardId && p.Owner == userId, cancellationToken);
    }
}