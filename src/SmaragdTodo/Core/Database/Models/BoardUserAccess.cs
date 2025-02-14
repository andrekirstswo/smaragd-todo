using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Core.Database.Models;

public class BoardUserAccess
{
    [JsonPropertyName("userId")]
    [JsonProperty("userId")]
    public string UserId { get; set; }

    [JsonPropertyName("role")]
    [JsonProperty("role")]
    public string Role { get; set; }
}

public static class BoardUserAccessRoles
{
    public const string Reader = "Reader";
    public const string Writer = "Writer";
    public const string Admin = "Admin";
}