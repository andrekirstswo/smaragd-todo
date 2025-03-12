using FluentValidation;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
{
    public CreateBoardCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotNull()
            .WithMessage("Name is required")
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(256)
            .WithMessage("Maximum length is 256");

    }
}