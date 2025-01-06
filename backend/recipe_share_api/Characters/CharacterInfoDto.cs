using recipe_share_api.EntityFramework;

namespace recipe_share_api.Characters;

public class CharacterInfoDto(BnetCharacter character)
{
    public string Name { get; set; } = character.Name;
    public string RealmSlug { get; set; } = character.BnetRealm.Slug;
}
