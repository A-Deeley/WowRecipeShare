namespace recipe_share_api.Characters;

public class CraftSkill
{
    public string Name { get; set; } = null!;
    public long CurrentExp { get; set; }
    public long MaxExp { get; set; }
    public List<SkillItem> Items { get; set; } = [];
}