using Core.Database.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository;

namespace Api.Database;

public class CredentialRepository
{
    private readonly IRepository<Credential> _repository;

    public CredentialRepository(IRepository<Credential> repository)
    {
        _repository = repository;
    }

    public ValueTask<Credential> CreateAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        return _repository.CreateAsync(credential, cancellationToken);
    }

    public async ValueTask<Credential?> GetByAccessToken(string accessToken, CancellationToken cancellationToken = default)
    {
        var existsCredential = await _repository.ExistsAsync(p => p.AccessToken == accessToken, cancellationToken: cancellationToken);

        if (!existsCredential)
        {
            return null;
        }

        var credentials = await _repository.GetAsync(p => p.AccessToken == accessToken, cancellationToken: cancellationToken);
        return credentials.First();
    }

    public async ValueTask<Credential?> GetByUserId(string userId, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId ORDER BY c.createdTimeUtc DESC OFFSET 1 LIMIT 1")
            .WithParameter("@userId", userId);
        var credentials = await _repository.GetByQueryAsync(query, cancellationToken);
        return credentials.FirstOrDefault();
    }
}