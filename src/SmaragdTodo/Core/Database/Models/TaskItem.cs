using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;

namespace Core.Database.Models;

[PartitionKeyPath("/boardId")]
public class TaskItem : FullItem
{
    public string TaskId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public List<string> AssignedTo { get; set; } = new List<string>();

    [JsonProperty("boardId")]
    public string BoardId { get; set; } = default!;

    protected override string GetPartitionKeyValue() => BoardId;
}