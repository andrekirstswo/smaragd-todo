using Core;
using Core.Models;
using MediatR;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardRequest : IRequest<Result<CreateBoardResponseDto, Error>>
{
    public string Name { get; set; } = default!;
}