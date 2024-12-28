namespace recipe_share_api.Characters;

public class ApiCharRef(CharacterInfo charInfo, ProfessionSkills? professions = null)
{
    public ProfessionSkills? Professions { get; set; } = professions;
    public CharacterInfo CharInfo { get; set; } = charInfo;
}

#region CraftSkill

#endregion
#region TradeSkill

#endregion
