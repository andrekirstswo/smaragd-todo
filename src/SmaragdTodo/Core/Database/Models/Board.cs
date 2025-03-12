using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace Core.Database.Models;

[PartitionKeyPath("/id")]
public class Board : FullItem
{
    public string Name { get; set; } = default!;
    public List<BoardUserAccess>? Accesses { get; set; } = new List<BoardUserAccess>();
    public string Owner { get; set; } = default!;
    public List<BoardSection>? Sections { get; set; } = new List<BoardSection>();
}
