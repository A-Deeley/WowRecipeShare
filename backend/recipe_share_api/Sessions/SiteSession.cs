using recipe_share_api.BattleNetApiResponses;
using System.IdentityModel.Tokens.Jwt;

namespace recipe_share_api.Sessions;

public class SiteSession(TokenResponseContent tokenResponse, ProfileUserWowResponse profileInfo)
{
    public int AccountId { get; set; } = profileInfo.id;
    public string BattleTag { get; set; } = ExtractBattleTag(tokenResponse.id_token);
    public List<int> OwnedCharacterIds { get; set; } = profileInfo.wow_accounts.SelectMany(wa => wa.characters.Select(c => c.id)).ToList();
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
