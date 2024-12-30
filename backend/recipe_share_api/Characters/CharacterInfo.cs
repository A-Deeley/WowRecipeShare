using recipe_share_api.BattleNetApiResponses;

namespace recipe_share_api.Characters;

public class CharacterInfo
{
    public CharacterInfo(int id, string name, string realm, string? guild = null)
    {
        CharacterId = id;
        RealmSlug = realm.ToLower();
        Name = name;
        GuildName = guild;
    }

    public CharacterInfo(Character character)
    {
        CharacterId = character.id;
        RealmSlug = character.realm.slug;
        Name = character.name;
        GuildName = character.guild?.name;
    }

    public string? GuildName { get; set; }
    public string Name { get; set; }
    public string RealmSlug { get; set; }
    public int CharacterId { get; set; }
}
