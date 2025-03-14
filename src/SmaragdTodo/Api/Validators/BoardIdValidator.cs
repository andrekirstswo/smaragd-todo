using Api.Extensions;
using Core.Database.Models;
using ErrorHandling;
using FluentValidation;
using Microsoft.Azure.CosmosRepository;

namespace Api.Validators;

public sealed class BoardIdValidator : AbstractValidator<BoardIdValidatorParameters>
{
    public BoardIdValidator(
        IReadOnlyRepository<Board> boardRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;

        RuleFor(r => r.BoardId)
            .MustAsync(async (boardId, token) =>
            {
                return await boardRepository.ExistsAsync(p => p.BoardId == boardId, cancellationToken: token);
            })
            .WithMessage(validation => KnownErrors.Board.NotFoundById(validation.BoardId).Message)
            .WithErrorCode(ErrorCodes.Board.NotFoundById);

        RuleFor(r => r.BoardId)
            .MustAsync(async (boardId, token) =>
            {
                ArgumentNullException.ThrowIfNull(httpContext);

                var userId = httpContext.User.GetUserId();

                return await boardRepository.ExistsAsync(p =>
                    p.BoardId == boardId &&
                    p.Owner == userId || (p.Accesses != null && p.Accesses.Any(a => a.UserId == userId)), cancellationToken: token);
            })
            .WithMessage(KnownErrors.Board.AccessDenied().Message)
            .WithErrorCode(ErrorCodes.Board.AccessDenied);
    }
}