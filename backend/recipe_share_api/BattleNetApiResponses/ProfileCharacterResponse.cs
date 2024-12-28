namespace recipe_share_api.BattleNetApiResponses;

public class ProfileCharacterResponse
{
    public Links _links { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public Gender gender { get; set; }
    public Faction faction { get; set; }
    public PlayableRace race { get; set; }
    public PlayableClass character_class { get; set; }
    public ActiveSpec active_spec { get; set; }
    public Realm realm { get; set; }
    public Guild guild { get; set; }
    public int level { get; set; }
    public int experience { get; set; }
    public Titles titles { get; set; }
    public PvpSummary pvp_summary { get; set; }
    public Media media { get; set; }
    public long last_login_timestamp { get; set; }
    public int average_item_level { get; set; }
    public int equipped_item_level { get; set; }
    public Specializations specializations { get; set; }
    public Statistics statistics { get; set; }
    public Equipment equipment { get; set; }
    public Appearance appearance { get; set; }
}