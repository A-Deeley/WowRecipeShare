namespace recipe_share_api.BattleNetApiResponses;

public class ProfileUserWowResponse
{
    public Links _links { get; set; }
    public int id { get; set; }
    public WowAccounts[] wow_accounts { get; set; }
}
