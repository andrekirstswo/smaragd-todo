using System.Text.Json.Serialization;

namespace Core.Database.Models;

public class Board : BaseEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("accesses")]
    public List<BoardUserAccess> Accesses { get; set; } = new List<BoardUserAccess>();

    [JsonPropertyName("owner")]
    public string Owner { get; set; }
}