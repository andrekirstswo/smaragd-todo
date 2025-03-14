using Api.Validators;
using FluentValidation;

namespace Api.Features.Board.GetBoardNameById;

public class GetBoardNameByIdQueryValidator : AbstractValidator<GetBoardNameByIdQuery>
{
    public GetBoardNameByIdQueryValidator(BoardIdValidator boardIdValidator)
    {
        RuleFor(r => new BoardIdValidatorParameters(r.BoardId))
            .SetValidator(boardIdValidator);
    }
}