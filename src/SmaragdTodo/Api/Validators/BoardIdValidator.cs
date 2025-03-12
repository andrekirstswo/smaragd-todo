using Api.Extensions;
using Core.Database.Models;
using Core.Models.ValueObjects;
using FluentValidation;
using Microsoft.Azure.CosmosRepository;

namespace Api.Validators;

public sealed class BoardIdValidator : AbstractValidator<BoardId>
{
    public BoardIdValidator(
        IReadOnlyRepository<Board> boardRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;

        RuleFor(r => r.Value)
            .MustAsync(async (id, token) =>
            {
                return await boardRepository.ExistsAsync(p => p.Id == id, cancellationToken: token);
            })
            .WithMessage(id => KnownErrors.Board.NotFoundById(id).Message)
            .WithErrorCode(ErrorCodes.Board.NotFoundById);

        RuleFor(r => r.Value)
            .MustAsync(async (id, token) =>
            {
                ArgumentNullException.ThrowIfNull(httpContext);

                // TODO check in Middleware/Pipeline when user not exists
                var userId = httpContext.User.UserId();

                return await boardRepository.ExistsAsync(p =>
                    p.Id == id &&
                    p.Owner == userId || (p.Accesses != null && p.Accesses.Any(a => a.UserId == userId)), cancellationToken: token);
            })
            .WithMessage(KnownErrors.Board.AccessDenied.Message)
            .WithErrorCode(ErrorCodes.Board.AccessDenied);
    }
}