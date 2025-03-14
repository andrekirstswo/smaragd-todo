﻿using Api.Validators;
using FluentValidation;

namespace Api.Features.Board.GetBoardNameById;

public class GetBoardNameByIdQueryValidator : AbstractValidator<GetBoardNameByIdQuery>
{
    public GetBoardNameByIdQueryValidator(BoardIdValidator boardIdValidator)
    {
        RuleFor(r => r.BoardId)
            .SetValidator(boardIdValidator);
    }
}