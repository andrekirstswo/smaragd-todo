using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Core.Database.Models;

public class Board : BaseEntity
{
    [JsonPropertyName("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("accesses")]
    [JsonProperty("accesses")]
    public IList<BoardUserAccess> Accesses { get; set; } = default!;

    [JsonPropertyName("owner")]
    [JsonProperty("owner")]
    public string Owner { get; set; } = default!;
}