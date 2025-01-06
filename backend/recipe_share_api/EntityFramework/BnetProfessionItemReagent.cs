namespace recipe_share_api.EntityFramework;

public class BnetProfessionItemReagent : Entity
{
    public long Count { get; set; }
    public string Name { get; set; } = null!;
    public int BnetItemId { get; set; }
    public virtual BnetItem BnetItem { get; set; } = null!;
    public int BnetProfessionItemId { get; set; }
    public virtual BnetProfessionItem BnetProfessionItem { get; set; } = null!;
}
