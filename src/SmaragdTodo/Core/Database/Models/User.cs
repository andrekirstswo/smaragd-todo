using System.Text.Json.Serialization;

namespace Core.Database.Models;

public class User : BaseEntity
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("name")]
    public Name? Name { get; set; }
    
    [JsonPropertyName("picture")]
    public string? Picture { get; set; }

    [JsonPropertyName("authenticationProvider")]
    public string AuthenticationProvider { get; set; } = default!;
}

public class Name
{
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
}