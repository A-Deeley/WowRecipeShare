namespace recipe_share_api.EntityFramework;

public class BnetUser : Entity
{
    public DateTime LastLogon { get; set; }
    public int AccountUniqueId { get; set; }
    public string BattleTag { get; set; } = null!;
}
