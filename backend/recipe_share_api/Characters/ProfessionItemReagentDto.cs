using recipe_share_api.EntityFramework;

namespace recipe_share_api.Characters;

public class ProfessionItemReagentDto(BnetProfessionItemReagent reagent)
{
    public int ItemId { get; set; } = reagent.BnetItemId;
    public string Name { get; set; } = reagent.Name;
    public long Count { get; set; } = reagent.Count;
}
