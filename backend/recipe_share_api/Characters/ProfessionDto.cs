using recipe_share_api.EntityFramework;

namespace recipe_share_api.Characters;

public class ProfessionDto(BnetProfession profession)
{
    public string Name { get; set; } = profession.Name;
    public string? SubSpecialisation { get; set; } = profession.SubSpecialisation;
    public long CurrentExp { get; set; } = profession.CurrentExp;
    public long MaxExp { get; set; } = profession.MaxExp;
    public IEnumerable<ProfessionItemDto> Items {  get; set; } = profession.BnetProfessionItems.Select(e => new ProfessionItemDto(e));
}
