using Microsoft.AspNetCore.Mvc;

namespace recipe_share_api.Controllers;

[ApiController]
[Route("[controller]")]
public class CharacterController : Controller
{
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(WowUserController._ram.Where(kvPair => kvPair.Value.Professions is not null).Select(kvPair => new { realm = kvPair.Value.CharInfo.RealmSlug, character = kvPair.Value.CharInfo.Name, id = kvPair.Key }));
    }

    [HttpGet("{characterId}")]
    public ActionResult<IEnumerable<TradeSkill>> GetCharacter(int characterId)
    {
        return Ok(WowUserController._ram[characterId]);
    }
}
