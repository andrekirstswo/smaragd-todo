using Api.Infrastructure;
using Core.Models;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardCommand : Command<CreateBoardResponseDto>
{
    public string Name { get; set; } = default!;
}