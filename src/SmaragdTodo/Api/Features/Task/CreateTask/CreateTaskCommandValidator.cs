using Api.Database;
using Api.Extensions;
using Api.Validators;
using Core.Database.Models;
using FluentValidation;

namespace Api.Features.Task.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator(
        BoardIdValidator boardIdValidator,
        BoardRepository boardRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        RuleFor(r => new BoardIdValidatorParameters(r.BoardId))
            .SetValidator(boardIdValidator);

        RuleFor(r => r.Model)
            .NotNull()
            .WithMessage(command => KnownErrors.Task.HasNoTitle(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoTitle)
            .NotEmpty()
            .WithMessage(command => KnownErrors.Task.HasNoTitle(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoTitle);

        RuleFor(r => r.Model.AssignedTo)
            .NotNull()
            .WithMessage(command => KnownErrors.Task.HasNoAssignee(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoAssignee)
            .NotEmpty()
            .WithMessage(command => KnownErrors.Task.HasNoAssignee(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoAssignee);

        RuleFor(r => r)
            .MustAsync(async (command, token) =>
            {
                ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                var isOwner = await boardRepository.IsUserOwnerOfBoardByIdAsync(command.BoardId, userId, token);

                if (isOwner)
                {
                    return true;
                }

                // TODO Eventuell eine extra Methode für das Holen der Accesses
                var board = await boardRepository.GetByIdAsync(command.BoardId, token);

                ArgumentNullException.ThrowIfNull(board);

                if (board.Accesses is null || board.Accesses.Count == 0)
                {
                    return false;
                }

                var access = board.Accesses.FirstOrDefault(p => p.UserId == userId);

                return access?.Role is BoardUserAccessRoles.Admin or BoardUserAccessRoles.Writer;
            })
            .WithMessage(KnownErrors.Board.AccessDenied().Message)
            .WithErrorCode(ErrorCodes.Board.AccessDenied);
    }
}