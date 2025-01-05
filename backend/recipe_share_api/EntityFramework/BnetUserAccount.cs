namespace recipe_share_api.EntityFramework;

public class BnetUserAccount : Entity
{
    public int UserId { get; set; }
    public virtual BnetUser User { get; set; } = null!;
    public virtual ICollection<BnetCharacter> BnetCharacters { get; set; } = [];
}
