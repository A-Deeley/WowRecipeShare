using recipe_share_api.EntityFramework;

namespace recipe_share_api.Login;

public class AccountDto(BnetUserAccount account)
{
    public IEnumerable<CharacterDto> Characters { get; } = account.BnetCharacters.Select(c => new CharacterDto(c));
}
