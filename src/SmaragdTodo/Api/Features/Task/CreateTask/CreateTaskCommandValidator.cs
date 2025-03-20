using Api.Database;
using Api.Validators;
using Core.Database.Models;
using Core.Extensions;
using ErrorHandling;
using FluentValidation;
using ErrorCodes = ErrorHandling.ErrorCodes;

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

        RuleFor(r => r.Model.Title)
            .NotNull()
            .WithMessage(command => KnownErrors.Task.HasNoTitle(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoTitle)
            .NotEmpty()
            .WithMessage(command => KnownErrors.Task.HasNoTitle(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoTitle)
            .OverridePropertyName(nameof(CreateTaskCommand.Model.Title));

        RuleFor(r => r.Model.AssignedTo)
            .NotNull()
            .WithMessage(command => KnownErrors.Task.HasNoAssignee(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoAssignee)
            .NotEmpty()
            .WithMessage(command => KnownErrors.Task.HasNoAssignee(command.BoardId).Message)
            .WithErrorCode(ErrorCodes.Task.HasNoAssignee)
            .OverridePropertyName(nameof(CreateTaskCommand.Model.Title));

        RuleFor(r => r)
            .MustAsync(async (command, token) =>
            {
                ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

                var userId = httpContextAccessor.HttpContext.User.GetUserId();

                return await boardRepository.IsUserOwnerOfBoardByIdAsync(command.BoardId, userId, token) ||
                       await boardRepository.IsUserInRolesAsync(command.BoardId, userId, new[] { BoardUserAccessRoles.Admin, BoardUserAccessRoles.Writer });
            })
            .WithMessage(KnownErrors.Board.AccessDenied().Message)
            .WithErrorCode(ErrorCodes.Board.AccessDenied)
            .OverridePropertyName(nameof(CreateTaskCommand.Model.Title));
    }
}