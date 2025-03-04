using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Core.Database.Models;

public class BaseEntity
{
    [Key]
    [JsonPropertyName("id")]
    [JsonProperty("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("createdAt")]
    [JsonProperty("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}