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
        if (!await _repository.ExistsAsync(boardId, cancellationToken: cancellationToken))
        {
            return null;
        }

        return await _repository.GetAsync(boardId, cancellationToken: cancellationToken);
    }

    public async Task<bool> IsUserOwnerOfBoardByIdAsync(string boardId, string userId, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(p => p.BoardId == boardId && p.Owner == userId, cancellationToken);
    }
}