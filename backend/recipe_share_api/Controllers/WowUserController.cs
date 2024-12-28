using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NLua;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace recipe_share_api.Controllers;

[ApiController]
[Route("[controller]")]
public class WowUserController(ISessionState sessionState, ProfileBattleNetClient bnetClient) : ControllerBase
{
    internal static Dictionary<int, ApiCharRef> _ram = new()
    {
        { -1,
        new(new(-1, "testchar", "dreamscythe", "TestGuildInc."), new() {
            TradeSkills = [new() {
         CurrentExp = 155,
         MaxExp = 175,
         Name = "Blacksmithing",
         Items = [
             new() {
                  Name = "Lionheart Helm",
                  Difficulty = Difficulty.Optimal,
                  ItemId = 12640,
                  HeaderName = "Armour",
                  Reagents = [
                      new() { ItemId = 12359, Name = "Thorium Bar", Count = 80 },
                      new() { ItemId = 12360, Name = "Arcanite Bar", Count = 12 },
                      new() { ItemId = 8146, Name = "Wicked Claw", Count = 40 },
                      new() { ItemId = 12361, Name = "Blue Sapphire", Count = 10 },
                      new() { ItemId = 12800, Name= "Azerothian Diamond", Count = 4 }
                      ]
             }
             ]
            }]
        }) }
    };

    [HttpGet]
    public async Task<ActionResult<ProfileUserWowResponse>> GetUserWow()
    {
        Request.Headers.TryGetValue(ProfileController.SessionIdHeader, out StringValues value);
        var sessionIdHeaderValue = value.FirstOrDefault();

        if (sessionIdHeaderValue is not string sessionId)
            return Unauthorized();

        if (sessionState.Session is null || sessionState.Session?.SessionId != sessionId)
            return Unauthorized();


        var accessToken = sessionState.Session.AccessToken;
        var profileInfo = await bnetClient.GetWowUser(accessToken);

        if (profileInfo is null)
            return NotFound();

        return Ok(profileInfo);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileCharacterResponse>> GetProtectedCharacter(int id)
    {
        Request.Headers.TryGetValue(ProfileController.SessionIdHeader, out StringValues value);
        var sessionIdHeaderValue = value.FirstOrDefault();

        if (sessionIdHeaderValue is not string sessionId)
            return Unauthorized();

        if (sessionState.Session is null || sessionState.Session?.SessionId != sessionId)
            return Unauthorized();



        var accessToken = sessionState.Session.AccessToken;
        var profileInfo = await bnetClient.GetCharacter(id, accessToken);

        if (profileInfo is null)
            return NotFound();

        return Ok(profileInfo);
    }

    [HttpGet("{characterId}/Profession")]
    public ActionResult GetProfessionData(int characterId)
    {
        if (!_ram.ContainsKey(characterId)) return NotFound($"No profession data for character id {characterId}");

        return Ok(JsonSerializer.Serialize(_ram[characterId]));
    }

    [HttpPost("{characterId}/Profession")]
    public ActionResult UploadProfessionData(int characterId, IFormFile file)
    {
        Request.Headers.TryGetValue(ProfileController.SessionIdHeader, out StringValues value);
        var sessionIdHeaderValue = value.FirstOrDefault();

        if (sessionIdHeaderValue is not string sessionId)
            return Unauthorized();

        if (sessionState.Session is null || sessionState.Session?.SessionId != sessionId)
            return Unauthorized();

        bool isOwnedByRequester = sessionState.Session?.OwnedCharacterIds.Contains(characterId) ?? false;
        if (!isOwnedByRequester)
            return Unauthorized();

        string fileContents = "";
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            fileContents = reader.ReadToEnd();
        }
        if (fileContents == "") return BadRequest("Empty file");
        Lua lua = new();
        lua.DoString(fileContents);
        LuaTable rawTradeskills = (LuaTable)lua["RecipeShareTradeskills"];
        List<TradeSkill> tradeSkills = [];
        LuaTable rawCraftskills = (LuaTable)lua["RecipeShareCraftskills"];
        List<CraftSkill> craftSkills = [];

        try
        {
            // Foreach profession (e.g: tailoring)
            foreach (LuaTable tradeskill in rawTradeskills.Values)
            {
                var levelInfo = (LuaTable)tradeskill["level"];
                TradeSkill t = new()
                {
                    Name = (string)tradeskill["name"],
                    CurrentExp = (Int64)levelInfo["current"],
                    MaxExp = (Int64)levelInfo["max"]
                };

                // Foreach header (category) of item
                foreach (LuaTable tradeskillInfo in ((LuaTable)tradeskill["items"]).Values)
                {
                    string headerName = (string)tradeskillInfo["title"];
                    // Foreach craftable item
                    foreach (LuaTable headerSkillItem in ((LuaTable)tradeskillInfo["items"]).Values)
                    {
                        Enum.TryParse((string)headerSkillItem["difficulty"], true, out Difficulty difficulty);
                        TradeSkillItem item = new()
                        {
                            Name = (string)headerSkillItem["name"],
                            HeaderName = headerName,
                            Difficulty = difficulty,
                            ItemId = int.Parse(((string)headerSkillItem["link"]).Split(':')[1])
                        };

                        foreach (LuaTable reagentinfo in ((LuaTable)headerSkillItem["reagents"]).Values)
                        {
                            SkillReagent reagent = new()
                            {
                                Name = (string)reagentinfo["name"],
                                Count = (Int64)reagentinfo["count"],
                                ItemId = int.Parse(((string)reagentinfo["link"]).Split(':')[1])
                            };

                            item.Reagents.Add(reagent);
                        }

                        t.Items.Add(item);
                    }

                }
                tradeSkills.Add(t);
            }
        }
        catch(Exception e)
        {
            return BadRequest($"Malformed file. {e.Message}");
        }

        try
        {
            // Foreach profession (e.g: enchanting)
            foreach (LuaTable tradeskill in rawCraftskills.Values)
            {
                var levelInfo = (LuaTable)tradeskill["level"];
                CraftSkill t = new()
                {
                    Name = (string)tradeskill["name"],
                    CurrentExp = (Int64)levelInfo["current"],
                    MaxExp = (Int64)levelInfo["max"]
                };

                // Foreach header (category) of item
                foreach (LuaTable tradeskillInfo in ((LuaTable)tradeskill["items"]).Values)
                {
                    Enum.TryParse((string)tradeskillInfo["difficulty"], true, out Difficulty difficulty);
                    SkillItem item = new()
                    {
                        Name = (string)tradeskillInfo["name"],
                        Difficulty = difficulty,
                        ItemId = int.Parse(((string)tradeskillInfo["link"]).Split('|')[2].Split(':')[1])
                    };

                    foreach (LuaTable reagentinfo in ((LuaTable)tradeskillInfo["reagents"]).Values)
                    {
                        SkillReagent reagent = new()
                        {
                            Name = (string)reagentinfo["name"],
                            Count = (Int64)reagentinfo["count"],
                            ItemId = int.Parse(((string)reagentinfo["link"]).Split(':')[1])
                        };

                        item.Reagents.Add(reagent);
                    }

                    t.Items.Add(item);

                }
                craftSkills.Add(t);
            }
        }
        catch (Exception e)
        {
            return BadRequest($"Malformed file. {e.Message}");
        }

        if (_ram.ContainsKey(characterId))
            _ram[characterId].Professions = new() { TradeSkills = tradeSkills, CraftSkills = craftSkills };

        //_ram.Add(characterId, new() { TradeSkills = tradeSkills, CraftSkills = craftSkills });

        return Ok();
    }
}

public class CharacterInfo(int id, string name, string realm, string? guild = null)
{
    public string? GuildName { get; set; } = guild;
    public string Name { get; set; } = name;
    public string RealmSlug { get; set; } = realm;
    public int CharacterId { get; set; } = id;
}

public class ApiCharRef(CharacterInfo charInfo, ProfessionSkills? professions = null)
{
    public ProfessionSkills? Professions { get; set; } = professions;
    public CharacterInfo CharInfo { get; set; } = charInfo;
}

public class ProfessionSkills
{
    public IEnumerable<TradeSkill> TradeSkills { get; set; } = [];
    public IEnumerable<CraftSkill> CraftSkills { get; set; } = [];
}
#region CraftSkill
public class CraftSkill
{
    public string Name { get; set; } = null!;
    public Int64 CurrentExp { get; set; }
    public Int64 MaxExp { get; set; }
    public List<SkillItem> Items { get; set; } = [];
}
#endregion


#region TradeSkill
public class TradeSkill
{
    public string Name { get; set; } = string.Empty;
    public Int64 CurrentExp { get; set; }
    public Int64 MaxExp { get; set; }
    public List<TradeSkillItem> Items { get; set; } = [];
}
#endregion

public class SkillItem
{
    public string Name { get; set; } = string.Empty;
    public int ItemId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<Difficulty>))]
    public Difficulty Difficulty { get; set; }
    public List<SkillReagent> Reagents { get; set; } = [];
}

public class TradeSkillItem : SkillItem
{
    public string HeaderName { get; set; } = string.Empty;
}

public class SkillReagent
{
    public string Name { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public Int64 Count { get; set; }
}

public enum Difficulty
{
    Trivial,
    Easy,
    Medium,
    Optimal,
    Difficult
}