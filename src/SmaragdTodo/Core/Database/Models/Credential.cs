using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;

namespace Core.Database.Models;

[PartitionKeyPath("/userId")]
public class Credential : FullItem
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public long? ExpiresInSeconds { get; set; }
    public string? IdToken { get; set; }
    public DateTimeOffset IssuedUtc { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    protected override string GetPartitionKeyValue() => UserId;
}