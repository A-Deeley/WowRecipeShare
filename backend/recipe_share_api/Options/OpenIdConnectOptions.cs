namespace recipe_share_api.Options;

public class OpenIdConnectOptions
{
    public const string Key = "OpenIdConnect";

    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}
