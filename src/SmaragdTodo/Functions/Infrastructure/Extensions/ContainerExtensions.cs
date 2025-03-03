using Microsoft.Azure.Cosmos;

namespace Functions.Infrastructure.Extensions;

public static class ContainerExtensions
{
    public static async Task<TResult?> Get<TResult, TEntity>(
        this Container container,
        string sql,
        Func<TEntity, TResult> select,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition(sql);

        if (parameters is not null)
        {
            foreach (var parameter in parameters)
            {
                query = query.WithParameter(parameter.Key, parameter.Value);
            }
        }

        using var resultSet = container.GetItemQueryIterator<TEntity>(query);

        if (!resultSet.HasMoreResults)
        {
            return default;
        }

        foreach (var item in await resultSet.ReadNextAsync(cancellationToken))
        {
            return select(item);
        }

        return default;
    }
}