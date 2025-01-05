using recipe_share_api.EntityFramework;

namespace recipe_share_api.Login;

public class CharacterDto(BnetCharacter character)
{
    public string Name { get; set; } = character.Name;
    public int Level { get; set; } = character.Level;
    public string Class { get; set; } = character.Class;
    public string Race { get; set; } = character.Race;
}
