namespace recipe_share_api.EntityFramework;

public class BnetUserAccount : Entity
{
    public int BnetUserId { get; set; }
    public virtual BnetUser BnetUser { get; set; } = null!;
    public virtual ICollection<BnetCharacter> BnetCharacters { get; set; } = [];
}
