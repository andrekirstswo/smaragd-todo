using Core.Models;

namespace WebUI.Pages.Board;

public class BoardCardViewModel
{
    public string BoardId { get; set; } = default!;
    public string Name { get; set; } = default!;

    public static BoardCardViewModel Map(GetBoardsDto dto) =>
        new BoardCardViewModel
        {
            BoardId = dto.BoardId,
            Name = dto.Name
        };
}