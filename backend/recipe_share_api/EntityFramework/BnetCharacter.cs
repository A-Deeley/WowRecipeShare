namespace recipe_share_api.EntityFramework;

public class BnetCharacter : Entity
{
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public string Class { get; set; } = null!;
    public string Race { get; set; } = null!;
    public int RealmId { get; set; }
    public int AccountId { get; set; }
    public virtual BnetUserAccount Account { get; set; } = null!;
    public virtual BnetRealm Realm { get; set; } = null!;
}
