using Core.Models;

namespace WebUI.Pages.Board;

public class BoardCardViewModel
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public static BoardCardViewModel Map(GetBoardsDto dto) =>
        new BoardCardViewModel
        {
            Id = dto.Id,
            Name = dto.Name
        };
}