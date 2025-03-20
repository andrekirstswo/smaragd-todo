// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Core.Models;

public class Board : BaseModel
{
    public string BoardId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public List<BoardSection> Sections { get; set; } = new List<BoardSection>();
    public List<BoardUserAccess> Accesses { get; set; } = new List<BoardUserAccess>();

    public List<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    public static Board Map(Database.Models.Board source)
    {
        return new Board
        {
            Name = source.Name,
            Accesses = source.Accesses?.Select(a => new BoardUserAccess(a.UserId, a.Role)).ToList() ?? new List<BoardUserAccess>(),
            InternalId = source.Id,
            BoardId = source.BoardId,
            Sections = source.Sections?.Select(s => new BoardSection
            {
                Name = s.Name,
                Order = s.Order,
                BoardSectionId = s.BoardSectionId
            }).ToList() ?? new List<BoardSection>()
        };
    }
}