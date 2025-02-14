namespace Api.Infrastructure;

public class GoogleAuthenticationOptions
{
    public string? ClientId { get; set; }
    public string? Authority { get; set; }
    public string? ValidIssuer { get; set; }
}