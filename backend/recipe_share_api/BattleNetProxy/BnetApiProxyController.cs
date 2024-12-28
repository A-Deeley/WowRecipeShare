using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using recipe_share_api.BattleNetApiResponses;
using recipe_share_api.Login;
using recipe_share_api.Sessions;

namespace recipe_share_api.BattleNetProxy;

[ApiController]
[Route("[controller]")]
public class BnetApiProxyController(ISessionState sessionState, ProfileBattleNetClient bnetClient) : ControllerBase
{


    [HttpGet("/profile/user/wow")]
    public async Task<ActionResult<ProfileUserWowResponse>> GetUserWow()
    {
        if (!sessionState.TryGetSession(Request, out var session) || session is null)
            return Unauthorized();

        var accessToken = session.AccessToken;
        var profileInfo = await bnetClient.GetWowUser(accessToken);

        if (profileInfo is null)
            return NotFound();

        return Ok(profileInfo);
    }

    [HttpGet("/profile/user/wow/protected-character/{realm}-{name}")]
    public async Task<ActionResult<ProfileCharacterResponse>> GetProtectedCharacter(string name, string realm)
    {
        if (!sessionState.TryGetSession(Request, out var session) || session is null)
            return Unauthorized();

        var accessToken = session.AccessToken;
        var profileInfo = await bnetClient.GetCharacter(name, realm, accessToken);

        if (profileInfo is null)
            return NotFound();

        return Ok(profileInfo);
    }
}