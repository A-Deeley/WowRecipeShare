using recipe_share_api.BattleNetApiResponses;
using recipe_share_api.EntityFramework;
using recipe_share_api.Sessions;

namespace recipe_share_api.Login;

public class PostLoginResponse(BnetUser user, Guid sessionId, SiteSession session)
{
    public int? PreferredAccountId { get; set; } = user.PreferredAccountId;
    public int? PreferredRealmId { get; set; } = user.PreferredRealmId;
    public string IdToken { get; set; } = session.IdToken;
    public Guid SessionId { get; set; } = sessionId;
    public DateTime ExpiresOn { get; set; } = session.ExpiresOn;
    public DateTime LastSyncedOn { get; set; } = user.LastBnetSync;
}
