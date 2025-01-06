
using Microsoft.Extensions.Options;
using recipe_share_api.Options;
using System.Net;
using System.Text;
using System.Text.Json;

namespace recipe_share_api.BattleNetApiResponses;

public class AuthenticationMiddleware(IOptions<OpenIdConnectOptions> options) : DelegatingHandler
{
    readonly OpenIdConnectOptions oidcConfig = options.Value;
    static TokenResponseContent? lastToken = null;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (lastToken is not null && DateTime.Now < DateTime.Now.AddSeconds(lastToken.expires_in))
        {
            request.Headers.Authorization = new("Bearer", lastToken.access_token);
        }
        else
            lastToken = null;

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            HttpClient client = new();
            MultipartFormDataContent content = new();
            StringContent stringContent = new("client_credentials");
            content.Add(stringContent, "grant_type");
            string unencodedBasic = $"{oidcConfig.ClientId}:{oidcConfig.ClientSecret}";
            var encodedBasic = Convert.ToBase64String(Encoding.UTF8.GetBytes(unencodedBasic));
            client.DefaultRequestHeaders.Authorization = new("Basic", encodedBasic);
            var authResponse = await client.PostAsync("https://oauth.battle.net/token", content);

            lastToken = await JsonSerializer.DeserializeAsync<TokenResponseContent>(authResponse.Content.ReadAsStream(cancellationToken), options: null, cancellationToken)
                ?? throw new InvalidOperationException("Failed to obtain application token in middleware.");
            request.Headers.Authorization = new("Bearer", lastToken.access_token);

        return await base.SendAsync(request, cancellationToken);
        }
        else
            return response;
    }
}
