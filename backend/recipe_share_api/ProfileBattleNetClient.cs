using recipe_share_api.Controllers;
using System.Text.Json;

namespace recipe_share_api;

public class ProfileBattleNetClient(HttpClient client)
{
    string baseUrl = "https://us.api.blizzard.com";
    string _params = "namespace=profile-classic1x-us&locale=en_US";

    public async Task<ProfileUserWowResponse?> GetWowUser(string token)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var bnetResponse = await client.GetAsync($"{baseUrl}/profile/user/wow?{_params}");

        string body = await bnetResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ProfileUserWowResponse>(body);
    }

    public async Task<ProfileCharacterResponse?> GetCharacter(int id, string token) 
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var hasCharInfo = WowUserController._ram.TryGetValue(id, out ApiCharRef? charRef);
        if (!hasCharInfo || charRef is null) return null;


        var bnetResponse = await client.GetAsync($"{baseUrl}/profile/wow/character/{charRef.CharInfo.RealmSlug}/{charRef.CharInfo.Name.ToLower()}?{_params}");

        string body = await bnetResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ProfileCharacterResponse>(body);
    }
}



#region GetCharacter

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



public class ActiveSpec
{
    public Key key { get; set; }
    public int id { get; set; }
}

public class Guild
{
    public Key key { get; set; }
    public string name { get; set; }
    public int id { get; set; }
    public Realm realm { get; set; }
    public Faction faction { get; set; }
}

public class Titles
{
    public string href { get; set; }
}

public class PvpSummary
{
    public string href { get; set; }
}

public class Media
{
    public string href { get; set; }
}

public class Specializations
{
    public string href { get; set; }
}

public class Statistics
{
    public string href { get; set; }
}

public class Equipment
{
    public string href { get; set; }
}

public class Appearance
{
    public string href { get; set; }
}

#endregion


#region GetWowUser
public class ProfileUserWowResponse
{
    public Links _links { get; set; }
    public int id { get; set; }
    public WowAccounts[] wow_accounts { get; set; }
}

public class Links
{
    public Self self { get; set; }
    public User user { get; set; }
    public Profile profile { get; set; }
}

public class User
{
    public string href { get; set; }
}

public class Profile
{
    public string href { get; set; }
}

public class WowAccounts
{
    public int id { get; set; }
    public Character[] characters { get; set; }
}

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



public class PlayableClass
{
    public Key key { get; set; }
    public string name { get; set; }
    public int id { get; set; }
}


public class PlayableRace
{
    public Key key { get; set; }
    public string name { get; set; }
    public int id { get; set; }
}





#endregion

#region Shared
public class Realm
{
    public Key key { get; set; }
    public string name { get; set; }
    public int id { get; set; }
    public string slug { get; set; }
}

public class Key
{
    public string href { get; set; }
}
public class Gender
{
    public string type { get; set; }
    public string name { get; set; }
}
public class Faction
{
    public string type { get; set; }
    public string name { get; set; }
}
public class Self
{
    public string href { get; set; }
}
#endregion