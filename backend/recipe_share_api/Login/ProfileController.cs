﻿using Microsoft.AspNetCore.Authorization;
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
                .FirstOrDefaultAsync(e => e.AccountUniqueId == userInfo.id);
            if (user is null)
            {
                user = new()
                {
                    AccountUniqueId = userInfo.id,
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
            await db.SaveChangesAsync();

            if (_bnetRam.TryGetValue(userInfo.id, out ProfileUserWowResponse? value) && value is not null)
            {
                profileInfo = value;
            }
            else
            {
                profileInfo = await bnetProfile.GetWowUser(tokenResponse.access_token);
                if (profileInfo is null) throw new InvalidOperationException("Could not obtain account information during login.");
                _bnetRam.Add(userInfo.id, profileInfo);
            }

            var session = new SiteSession(contentResponse, profileInfo);
            var sessionId = sessionState.Start(session);
            
            return Ok(new { session_id = sessionId, id_token = session.IdToken, session.ExpiresOn });
        }

        return BadRequest(body);
    }
}