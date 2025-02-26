using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Core.Database.Models;

public class User : BaseEntity
{
    [JsonPropertyName("email")]
    [JsonProperty("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("name")]
    [JsonProperty("name")]
    public Name? Name { get; set; }
    
    [JsonPropertyName("picture")]
    [JsonProperty("picture")]
    public string? Picture { get; set; }

    [JsonPropertyName("authenticationProvider")]
    [JsonProperty("authenticationProvider")]
    public string AuthenticationProvider { get; set; } = default!;
}

public class Name
{
    [JsonPropertyName("firstName")]
    [JsonProperty("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    [JsonProperty("lastName")]
    public string? LastName { get; set; }
}