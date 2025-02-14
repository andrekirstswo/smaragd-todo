using Core;
using MediatR;

namespace Api.Features.Board.CreateBoard;

public class CreateBoardRequest : IRequest<Result<CreateBoardRequestResult, Error>>
{
    public string Name { get; set; } = default!;
}

public class CreateBoardRequestResult
{
    public string Url { get; set; } = default!;
}