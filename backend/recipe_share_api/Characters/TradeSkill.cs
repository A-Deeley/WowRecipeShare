namespace recipe_share_api.Characters;
public class TradeSkill
{
    public string Name { get; set; } = string.Empty;
    public long CurrentExp { get; set; }
    public long MaxExp { get; set; }
    public List<TradeSkillItem> Items { get; set; } = [];
    public string? SubSpecialisation { get; set; }
}
