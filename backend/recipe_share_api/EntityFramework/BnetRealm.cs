namespace recipe_share_api.EntityFramework;

public class BnetRealm : Entity
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public virtual ICollection<BnetCharacter> Characters { get; set; } = [];
    public virtual ICollection<BnetUser> Users { get; set; } = [];
}
