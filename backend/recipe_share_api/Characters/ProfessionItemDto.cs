using recipe_share_api.EntityFramework;

namespace recipe_share_api.Characters;

public class ProfessionItemDto(BnetProfessionItem item)
{
    public int ItemId { get; set; } = item.BnetItemId;
    public string Name { get; set; } = item.Name;
    public string? HeaderName { get; set; } = item.HeaderName;
    public string Difficulty { get; set; } = item.Difficulty;
    public ItemCooldownDto? ItemCooldown { get
        {
            if (item.Current is not null && item.Delta is not null)
                return new()
                {
                    Current = (long)item.Current,
                    Delta = (double)item.Delta
                };
            else
                return null;
        } 
    }
    public IEnumerable<ProfessionItemReagentDto> Reagents { get; set; } = item.BnetProfessionItemReagents.Select(e => new ProfessionItemReagentDto(e));
}
