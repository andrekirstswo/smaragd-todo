using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;
using Newtonsoft.Json;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Core.Database.Models;

[Container(ContainerNames.Users)]
[PartitionKeyPath("/authenticationProvider")]
public class User : FullItem
{
    public string UserId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public Name? Name { get; set; }
    public string? Picture { get; set; }

    [JsonProperty("authenticationProvider")]
    public string AuthenticationProvider { get; set; } = default!;

    protected override string GetPartitionKeyValue() => AuthenticationProvider;
}

public class Name
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}