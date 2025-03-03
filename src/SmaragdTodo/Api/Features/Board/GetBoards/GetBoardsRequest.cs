using Core;
using Core.Models;
using MediatR;

namespace Api.Features.Board.GetBoards;

public class GetBoardsRequest : IRequest<Result<List<GetBoardsDto>, Error>>
{
}