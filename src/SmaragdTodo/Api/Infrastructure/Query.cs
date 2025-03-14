using Core;
using ErrorHandling;
using MediatR;

namespace Api.Infrastructure;

public class Query<TResponse> : IRequest<Result<TResponse, Error>>
{
}

public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse, Error>>
    where TQuery : Query<TResponse>
{
    public abstract Task<Result<TResponse, Error>> Handle(TQuery request, CancellationToken cancellationToken);
}