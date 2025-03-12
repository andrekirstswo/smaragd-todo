namespace Core.Models;

public class GetBoardByIdDto
{
    public string BoardId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Owner { get; set; } = default!;
    public IEnumerable<BoardSection> Sections { get; set; } = new List<BoardSection>();
    public IEnumerable<BoardUserAccess> Accesses { get; set; } = new List<BoardUserAccess>();
}