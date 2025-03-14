using Microsoft.Azure.CosmosRepository;
using User = Core.Database.Models.User;

namespace Api.Database;

public class UserRepository
{
    private readonly IRepository<User> _repository;

    public UserRepository(IRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<bool> ExistsAsync(string authenticationProvider, string userId, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(p => p.AuthenticationProvider == authenticationProvider && p.UserId == userId, cancellationToken: cancellationToken);
    }

    public async ValueTask<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        return await _repository.CreateAsync(user, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(string authenticationProvider, string userId, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(p => p.AuthenticationProvider == authenticationProvider && p.UserId == userId, cancellationToken);

        return result.SingleOrDefault();
    }

    public async ValueTask<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        return await _repository.UpdateAsync(user, cancellationToken: cancellationToken);
    }
}