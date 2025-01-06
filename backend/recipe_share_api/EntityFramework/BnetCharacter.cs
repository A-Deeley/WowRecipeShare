namespace recipe_share_api.EntityFramework;

public class BnetCharacter : Entity
{
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public string Class { get; set; } = null!;
    public string Race { get; set; } = null!;
    public int BnetRealmId { get; set; }
    public int BnetUserAccountId { get; set; }
    public virtual ICollection<BnetProfession> BnetProfessions { get; set; } = null!;
    public virtual BnetUserAccount BnetUserAccount { get; set; } = null!;
    public virtual BnetRealm BnetRealm { get; set; } = null!;
}
