using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Core.Database.Models;

[PartitionKeyPath("/boardId")]
public class Board : FullItem
{
    [JsonProperty("boardId")]
    public string BoardId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public List<BoardUserAccess>? Accesses { get; set; } = new List<BoardUserAccess>();
    public string Owner { get; set; } = default!;
    public List<BoardSection>? Sections { get; set; } = new List<BoardSection>();
    protected override string GetPartitionKeyValue() => BoardId;
}