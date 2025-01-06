using recipe_share_api.EntityFramework;

namespace recipe_share_api.Characters;

public class CharacterListDto(BnetCharacter character)
{
    public string Character { get; set; } = character.Name;
    public string Realm { get; set; } = character.BnetRealm.Name;
    public int Id { get; set; } = (int)character.Id!;
}
