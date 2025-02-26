using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;

namespace Api.Services;

public interface IGoogleAuthHelper
{
    string[] GetScopes();
    string ScopeToString();

    ClientSecrets GetClientSecrets();
}

public class GoogleAuthHelper : IGoogleAuthHelper
{
    private readonly IConfiguration _configuration;

    public GoogleAuthHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string[] GetScopes() =>
        new[]
        {
            Oauth2Service.Scope.Openid,
            Oauth2Service.Scope.UserinfoEmail,
            Oauth2Service.Scope.UserinfoProfile
        };

    public string ScopeToString() => string.Join(", ", GetScopes());

    public ClientSecrets GetClientSecrets()
    {
        var section = _configuration.GetSection("Authentication:Google");
        
        var clientId = section["ClientId"];
        var clientSecret = section["ClientSecret"];
        
        return new ClientSecrets
        {
            ClientId = clientId,
            ClientSecret = clientSecret
        };
    }
}