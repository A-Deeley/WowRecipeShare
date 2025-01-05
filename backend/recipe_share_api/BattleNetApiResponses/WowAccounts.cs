using recipe_share_api.EntityFramework;

namespace recipe_share_api.BattleNetApiResponses;

public class WowAccounts
{
    public int id { get; set; }
    public Character[] characters { get; set; }

    public BnetUserAccount MapToBnetUser()
    {
        return new BnetUserAccount() 
        {
            Id = id, 
            BnetCharacters = characters.Select(c => c.MapToBnetCharacter()).ToList()
        };
    }
}
