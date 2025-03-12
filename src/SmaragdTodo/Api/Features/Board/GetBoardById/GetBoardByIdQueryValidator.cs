using Api.Validators;
using FluentValidation;

namespace Api.Features.Board.GetBoardById;

public class GetBoardByIdQueryValidator : AbstractValidator<GetBoardByIdQuery>
{
    public GetBoardByIdQueryValidator(BoardIdValidator boardIdValidator)
    {
        RuleFor(r => r.BoardId)
            .SetValidator(boardIdValidator);
    }
}