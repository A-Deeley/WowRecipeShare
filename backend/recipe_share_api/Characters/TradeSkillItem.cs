﻿namespace recipe_share_api.Characters;

public class TradeSkillItem : SkillItem
{
    public string HeaderName { get; set; } = string.Empty;
    public ItemCooldown? Cooldown { get; set; }
}
