namespace Core.Database.Models;

public class BoardSection
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = default!;
    public int Order { get; set; }
}