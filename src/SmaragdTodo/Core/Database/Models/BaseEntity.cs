using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Core.Database.Models;

public class BaseEntity
{
    // ReSharper disable once InconsistentNaming
    [Key, JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; set; }
}