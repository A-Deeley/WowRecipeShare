namespace recipe_share_api.BattleNetApiResponses;

public class Character
{
    public string name { get; set; }
    public int id { get; set; }
    public Realm realm { get; set; }
    public PlayableClass playable_class { get; set; }
    public PlayableRace playable_race { get; set; }
    public Gender gender { get; set; }
    public Faction faction { get; set; }
    public int level { get; set; }
}
