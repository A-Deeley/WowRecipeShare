using recipe_share_api.EntityFramework;

namespace recipe_share_api.Characters;

public class GetProfessionDataResponse(BnetCharacter character)
{
    public CharacterInfoDto CharInfo { get; set; } = new(character);
    public IEnumerable<ProfessionDto> Professions { get; set; } = character.BnetProfessions.Select(e => new ProfessionDto(e)) ?? [];
}
