namespace recipe_share_api.EntityFramework;

public class BnetRealm : Entity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public virtual ICollection<BnetCharacter> BnetCharacters { get; set; } = [];
    public virtual ICollection<BnetUser> BnetUsers { get; set; } = [];
}
