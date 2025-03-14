using Core;
using ErrorHandling;
using MediatR;

namespace Api.Infrastructure;

public class Command<TResponse> : IRequest<Result<TResponse, Error>>
{
}

public abstract class CommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse, Error>>
    where TCommand : Command<TResponse>
{
    public abstract Task<Result<TResponse, Error>> Handle(TCommand request, CancellationToken cancellationToken);
}