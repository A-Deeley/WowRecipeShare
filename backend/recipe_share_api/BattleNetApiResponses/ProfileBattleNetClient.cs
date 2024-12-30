using recipe_share_api.BattleNetProxy;
using recipe_share_api.Characters;
using recipe_share_api.Controllers;
using System.Text.Json;

namespace recipe_share_api.BattleNetApiResponses;

public class ProfileBattleNetClient(HttpClient client)
{
    readonly string baseUrl = "https://us.api.blizzard.com";
    readonly string _params = "namespace=profile-classic1x-us&locale=en_US";

    public async Task<UserInfoResponse?> GetUserInfo(string token)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var bnetResponse = await client.GetAsync("https://oauth.battle.net/userinfo");

        string body = await bnetResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoResponse>(body);
    }

    public async Task<ProfileUserWowResponse?> GetWowUser(string token)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var bnetResponse = await client.GetAsync($"{baseUrl}/profile/user/wow?{_params}");

        string body = await bnetResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ProfileUserWowResponse>(body);
    }

    public async Task<ProfileCharacterResponse?> GetCharacter(string name, string realm, string token)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);


        var bnetResponse = await client.GetAsync($"{baseUrl}/profile/wow/character/{realm.ToLower()}/{name.ToLower()}?{_params}");

        string body = await bnetResponse.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<ProfileCharacterResponse>(body);
    }
}
