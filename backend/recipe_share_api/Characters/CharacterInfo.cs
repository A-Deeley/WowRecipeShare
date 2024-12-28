namespace recipe_share_api.Characters;

public class CharacterInfo(int id, string name, string realm, string? guild = null)
{
    public string? GuildName { get; set; } = guild;
    public string Name { get; set; } = name;
    public string RealmSlug { get; set; } = realm;
    public int CharacterId { get; set; } = id;
}
