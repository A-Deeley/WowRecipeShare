namespace recipe_share_api.EntityFramework;

public class BnetUser : Entity
{
    public DateTime LastLogon { get; set; }
    public DateTime LastBnetSync { get; set; }
    public string BattleTag { get; set; } = null!;
    public virtual ICollection<BnetUserAccount> BnetUserAccounts { get; set; } = [];
    public int? PreferredAccountId { get; set; }
    //public virtual BnetUserAccount? PreferredAccount { get; set; }
    public int? PreferredRealmId { get; set; }
    //public virtual BnetRealm? PreferredRealm { get; set; }
}
