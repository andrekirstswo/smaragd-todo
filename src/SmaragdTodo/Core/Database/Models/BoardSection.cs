namespace Core.Database.Models;

public class BoardSection
{
    public string BoardSectionId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Order { get; set; }
}