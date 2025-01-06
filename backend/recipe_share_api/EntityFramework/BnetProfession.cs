namespace recipe_share_api.EntityFramework;

public class BnetProfession : Entity
{
    public string Name { get; set; } = null!;
    public long CurrentExp { get; set; }
    public long MaxExp { get; set; }
    public string? SubSpecialisation { get; set; }
    public virtual ICollection<BnetProfessionItem> BnetProfessionItems { get; set; } = [];
    public int BnetCharacterId { get; set; }
    public virtual BnetCharacter BnetCharacter { get; set; } = null!;
}
