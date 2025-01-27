using System.Text.Json.Serialization;

namespace Functions.Database.Model;

public class BoardEntity : BaseEntity
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
}