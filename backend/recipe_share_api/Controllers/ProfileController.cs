using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;

namespace recipe_share_api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfileController(ISessionState sessionState, IWebHostEnvironment webEnv, ProfileBattleNetClient bnetProfile) : ControllerBase
{
    static string id = "ef6da78b517e49c2a609812b995abc8e";
    static string secret = "RIl6iLmjR0kr9sx0bzP4K3kktHNj5vFw";
    public const string SessionIdHeader = "X-RecipeShare-SessionId";
    string redirectUri = webEnv.IsProduction() ? "https://recipeshare.kuronai.dev/login-bnet" : "http://localhost:5173/login-bnet";

    [HttpGet]
    public ActionResult Get()
    {
        bool hasSessionId = Request.Headers.TryGetValue(SessionIdHeader, out var sessionId);

        if (!hasSessionId) return Unauthorized();


        var t = User.Claims;
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> Login(PostLoginRequest request)
    {
        HttpClient client = new HttpClient();
        string unencodedBasic = $"{id}:{secret}";
        var encodedBasic = Convert.ToBase64String(Encoding.UTF8.GetBytes(unencodedBasic));
        client.DefaultRequestHeaders.Authorization = new("Basic", encodedBasic);
        Dictionary<string, string> parameters = new Dictionary<string, string>()
        {
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" },
            { "code", request.Code }
        };
        var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync("https://oauth.battle.net/token", content);
        string body = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponseContent>(body);
        if (tokenResponse is TokenResponseContent contentResponse)
        {
            var profileInfo = await bnetProfile.GetWowUser(tokenResponse.access_token);
            var ownedChars = profileInfo?.wow_accounts.SelectMany(wa => wa.characters) ?? [];
            var session = new SiteSession(contentResponse, ownedChars.Select(c => c.id));
            sessionState.Start(session);
            foreach (var _char in ownedChars)
            {
                WowUserController._ram[_char.id] = new(new(_char.id, _char.name, _char.realm.slug));
            }
            return Ok(new { session_id = session.SessionId, id_token = session.IdToken, contentResponse.expires_in });
        }

        return BadRequest(body);
    }
}


public class TokenResponseContent
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string scope { get; set; }
    public string sub { get; set; }
    public string id_token { get; set; }
}


public class SiteSession(TokenResponseContent tokenResponse, IEnumerable<int>? ownedCharactedIds)
{
    public string BattleTag { get; set; } = ExtractBattleTag(tokenResponse.id_token);
    public List<int> OwnedCharacterIds { get; set; } = ownedCharactedIds is not null ? new(ownedCharactedIds) : [];
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string AccessToken { get; set; } = tokenResponse.access_token;
    public DateTime ExpiresOn { get; set; } = DateTime.Now.AddSeconds(tokenResponse.expires_in);
    public string IdToken { get; set; } = tokenResponse.id_token;

    static string ExtractBattleTag(string idToken)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jsonToken = jwtHandler.ReadJwtToken(idToken);
        return jsonToken.Payload["battle_tag"] as string ?? "undf.";
    }
}

public class PostLoginRequest
{
    public string Code { get; set; }
}