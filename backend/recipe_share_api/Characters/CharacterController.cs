using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NLua;
using recipe_share_api.Characters;
using recipe_share_api.Exceptions;
using recipe_share_api.Exceptions.AddonExceptions;
using recipe_share_api.Login;
using recipe_share_api.Sessions;
using System.Text.Json;

namespace recipe_share_api.Controllers;

[ApiController]
[Route("[controller]")]
public class CharacterController(ISessionState sessionState) : Controller
{
    // TEMPORARY "DATABASE"
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
    public ActionResult Get()
    {
        return Ok(_ram.Where(kvPair => kvPair.Value.Professions is not null).Select(kvPair => new { realm = kvPair.Value.CharInfo.RealmSlug, character = kvPair.Value.CharInfo.Name, id = kvPair.Key }));
    }

    [HttpGet("{characterId}")]
    public ActionResult<IEnumerable<TradeSkill>> GetCharacter(int characterId)
    {
        return Ok(_ram[characterId]);
    }

    [HttpGet("{characterId}/Profession")]
    public ActionResult GetProfessionData(int characterId)
    {
        if (!_ram.TryGetValue(characterId, out ApiCharRef? value)) return NotFound($"No profession data for character id {characterId}");

        return Ok(JsonSerializer.Serialize(value));
    }

    [HttpPost("{characterId}/Profession")]
    public ActionResult UploadProfessionData(int characterId, IFormFile file)
    {
        if (!sessionState.TryGetSession(Request, out var session))
            return Unauthorized();

        bool isOwnedByRequester = session?.OwnedCharacterIds.Contains(characterId) ?? false;
        if (!isOwnedByRequester)
            return Unauthorized();

        var profileInfo = ProfileController._bnetRam[session!.AccountId];
        var charInfo = profileInfo.wow_accounts.SelectMany(wa => wa.characters).FirstOrDefault(c => c.id == characterId);
        if (charInfo is null) throw new InvalidOperationException("Could not find character");

        string fileContents = "";
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            fileContents = reader.ReadToEnd();
        }
        if (fileContents == "") return BadRequest("Empty file");
        Lua lua = new();
        lua.DoString(fileContents);

        LuaTable addonInfo = (LuaTable)lua["AddonInfo"];
        string charNameInFile = (string)addonInfo["name"];
        string realmNameInFile = (string)addonInfo["realm"];
        string addonVersionInFile = (string)addonInfo["version"];
        
        try
        {
            // Check if the user is uploading a valid file from the correct addon version.
            // Somehow get version from DB
            string supportedVersion = "0.1.7";
            if (addonVersionInFile != supportedVersion)
                throw new InvalidVersionException(addonVersionInFile, supportedVersion);

            // Check if the character in the file is the correct character
            if (!charNameInFile.Equals(charInfo.name, StringComparison.OrdinalIgnoreCase) || !realmNameInFile.Equals(charInfo.realm.name, StringComparison.OrdinalIgnoreCase))
                throw new InvalidCharacterException();
        }
        catch(RecipeShareException ex)
        {
            return BadRequest(new RecipeShareExceptionResponse(ex));
        }

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
                string? subspec = null;
                try
                {
                    subspec = (string)tradeskill["subspec"];
                }
                catch { }
                TradeSkill t = new()
                {
                    Name = (string)tradeskill["name"],
                    CurrentExp = (Int64)levelInfo["current"],
                    MaxExp = (Int64)levelInfo["max"],
                    SubSpecialisation = subspec
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
        catch (Exception e)
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
        else
        {
            CharacterInfo appCharInfo = new(charInfo);
            ProfessionSkills skills = new() { TradeSkills = tradeSkills, CraftSkills = craftSkills };
            _ram.Add(characterId, new(appCharInfo, skills));
        }

        return Ok();
    }
}
