namespace recipe_share_api.EntityFramework;

public class BnetProfessionItem : Entity
{
    public int BnetItemId { get; set; }
    public virtual BnetItem BnetItem { get; set; } = null!;
    public virtual ICollection<BnetProfessionItemReagent> BnetProfessionItemReagents { get; set; } = [];
    public string Name { get; set; } = null!;
    public string Difficulty { get; set; } = null!;
    public string? HeaderName { get; set; }
    public Int64? Current { get; set; }
    public double? Delta { get; set; }
}
