namespace recipe_share_api.EntityFramework;

public class BnetItem : Entity
{
    public string Name { get; set; } = null!;
    public string Quality { get; set; } = null!;
    public int ItemLevel { get; set; }
    public int RequiredLevel { get; set; }
    public int ItemClassId { get; set; }
    public string ItemClassName { get; set; } = null!;
    public int ItemSubclassId { get; set; }
    public string ItemSubclassName { get; set; } = null!;
}
