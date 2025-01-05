using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using recipe_share_api.BattleNetApiResponses;
using recipe_share_api.BattleNetProxy;
using recipe_share_api.Controllers;
using recipe_share_api.EntityFramework;
using recipe_share_api.Options;
using recipe_share_api.Sessions;
using System.Collections.Specialized;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;

namespace recipe_share_api.Login;

[ApiController]
[Route("[controller]")]
public class ProfileController(ISessionState sessionState, IWebHostEnvironment webEnv, ProfileBattleNetClient bnetProfile, IOptions<OpenIdConnectOptions> oidcOptions, RecipeShareDbContext db) : ControllerBase
{
    readonly OpenIdConnectOptions oidcConfig = oidcOptions.Value;
    readonly string redirectUri = webEnv.IsProduction() ? "https://recipeshare.kuronai.dev/login-bnet" : "http://localhost:5173/login-bnet";
    internal static Dictionary<int, ProfileUserWowResponse> _bnetRam = new();

    [HttpPost("{accountId}/sync")]
    public async Task<ActionResult> ForceSync(int accountId)
    {
        if (!sessionState.TryGetSession(Request, out var session))
            return Unauthorized();

        bool isOwnedByRequester = session!.AccountId == accountId;
        if (!isOwnedByRequester)
            return Unauthorized();

        var user = await db
            .BnetUsers
            .Include(e => e.Accounts)
                .ThenInclude(e => e.BnetCharacters)
            .FirstOrDefaultAsync(e => e.Id == accountId);

        if (user is null)
            return NotFound();

        var profileInfo = await bnetProfile.GetWowUser(session!.AccessToken);
        if (profileInfo is null) throw new InvalidOperationException("Error syncing with BattleNet during login.");
        user.Accounts = profileInfo.wow_accounts.Select(w => w.MapToBnetUser()).ToList();
        user.LastBnetSync = DateTime.Now;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch]
    public async Task<ActionResult> Update([FromBody]PatchBnetUserRequest request)
    {
        if (!sessionState.TryGetSession(Request, out var session))
            return Unauthorized();

        bool isOwnedByRequester = session!.AccountId == request.Id;
        if (!isOwnedByRequester)
            return Unauthorized();

        var user = await db
            .BnetUsers
            .FirstOrDefaultAsync(e => e.Id == request.Id);

        if (user is null)
            return NotFound();

        if (user.PreferredRealmId is not null && user.PreferredAccountId is null) return BadRequest("Must specify account id with realm id!");

        user.PreferredRealmId = request.PreferredRealmId;
        user.PreferredAccountId = request.PreferredAccountId;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> Login(PostLoginRequest request)
    {
        HttpClient client = new();
        string unencodedBasic = $"{oidcConfig.ClientId}:{oidcConfig.ClientSecret}";
        var encodedBasic = Convert.ToBase64String(Encoding.UTF8.GetBytes(unencodedBasic));
        client.DefaultRequestHeaders.Authorization = new("Basic", encodedBasic);
        Dictionary<string, string> parameters = new()
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

            ProfileUserWowResponse? profileInfo = null;
            var userInfo = await bnetProfile.GetUserInfo(tokenResponse.access_token) ?? throw new InvalidOperationException("could not fetch user info");
            var user = await db
                .BnetUsers
                .Include(e => e.Accounts)
                    .ThenInclude(e => e.BnetCharacters)
                .FirstOrDefaultAsync(e => e.Id == userInfo.id);
            if (user is null)
            {
                user = new()
                {
                    Id = userInfo.id,
                    BattleTag = userInfo.battletag,
                    LastLogon = DateTime.Now
                };
                await db.AddAsync(user);
            }
            else
            {
                user.LastLogon = DateTime.Now;
                user.BattleTag = userInfo.battletag;
            }
            
            if (user.LastBnetSync.AddDays(1) <= DateTime.Now)
            {
                profileInfo = await bnetProfile.GetWowUser(tokenResponse.access_token);
                if (profileInfo is null) throw new InvalidOperationException("Error syncing with BattleNet during login.");
                user.Accounts = profileInfo.wow_accounts.Select(w => w.MapToBnetUser()).ToList();
                user.LastBnetSync = DateTime.Now;
            }
            await db.SaveChangesAsync();

            var session = new SiteSession(contentResponse, user);
            var sessionId = sessionState.Start(session);
            var loginResponse = new PostLoginResponse(user, sessionId, session);
            
            return Ok(loginResponse);
        }

        return BadRequest(body);
    }
}