using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using NLua;
using recipe_share_api.Characters;
using recipe_share_api.EntityFramework;
using recipe_share_api.EntityFramework.Services;
using recipe_share_api.Exceptions;
using recipe_share_api.Exceptions.AddonExceptions;
using recipe_share_api.Login;
using recipe_share_api.Sessions;
using System.Text.Json;

namespace recipe_share_api.Controllers;

[ApiController]
[Route("[controller]")]
public class CharacterController(ISessionState sessionState, RecipeShareDbContext db, BnetItemService itemService) : Controller
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
    public async Task<ActionResult> Get()
    {
        var characters = await db
            .Characters
            .Include(e => e.BnetRealm)
            .Where(e => e.BnetProfessions.Count > 0)
            .Select(e => new CharacterListDto(e))
            .ToListAsync();

        return Ok(characters);
    }

    [HttpGet("{characterId}")]
    public async Task<ActionResult> GetCharacter(int characterId)
    {
        var character = await db
            .Characters
            .Include(e => e.BnetRealm)
            .Include(e => e.BnetProfessions)
                .ThenInclude(e => e.BnetProfessionItems)
                    .ThenInclude(e => e.BnetProfessionItemReagents)
            .FirstOrDefaultAsync(e => e.Id == characterId);
        if (character is null) return NotFound();
        if (character.BnetProfessions is null || character.BnetProfessions.Count == 0) return NotFound();

        var dto = new GetProfessionDataResponse(character);

        return Ok(dto);
    }

    [HttpGet("{characterId}/Profession")]
    public async Task<ActionResult> GetProfessionData(int characterId)
    {
        var character = await db
            .Characters
            .Include(e => e.BnetRealm)
            .Include(e => e.BnetProfessions)
                .ThenInclude(e => e.BnetProfessionItems)
                    .ThenInclude(e => e.BnetProfessionItemReagents)
            .FirstOrDefaultAsync(e => e.Id == characterId);
        if (character is null) return NotFound();
        if (character.BnetProfessions is null || character.BnetProfessions.Count == 0) return NotFound();

        var dto = new GetProfessionDataResponse(character);
        
        return Ok(dto);
    }

    [HttpPost("{characterId}/Profession")]
    public async Task<ActionResult> UploadProfessionData(int characterId, IFormFile file)
    {
        if (!sessionState.TryGetSession(Request, out var session))
            return Unauthorized();

        bool isOwnedByRequester = session?.OwnedCharacterIds.Contains(characterId) ?? false;
        if (!isOwnedByRequester)
            return Unauthorized();

        var user = await db
            .Users
            .Include(e => e.BnetUserAccounts)
                .ThenInclude(e => e.BnetCharacters)
                    .ThenInclude(e => e.BnetRealm)
            .FirstOrDefaultAsync(e => e.Id == session!.AccountId) ?? throw new InvalidOperationException("Could not find user");

        var charInfo = user.BnetUserAccounts.SelectMany(a => a.BnetCharacters).FirstOrDefault(c => c.Id == characterId);
        if (charInfo is null) return NotFound();
        
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
            string supportedVersion = (await db
                .ApplicationSettings
                .FirstAsync(e => e.Key == ApplicationSetting.AddonVersion)).Value;

            if (addonVersionInFile != supportedVersion)
                throw new InvalidVersionException(addonVersionInFile, supportedVersion);

            // Check if the character in the file is the correct character
            if (!charNameInFile.Equals(charInfo.Name, StringComparison.OrdinalIgnoreCase) || !realmNameInFile.Equals(charInfo.BnetRealm.Name, StringComparison.OrdinalIgnoreCase))
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
                        ItemCooldownDto? cd = null;
                        LuaTable cooldownInfo = (LuaTable)headerSkillItem["cooldown"];
                        if (cooldownInfo is not null)
                        {
                            cd = new()
                            {
                                Current = (Int64)cooldownInfo["current"],
                                Delta = (double)cooldownInfo["delta"]
                            };

                        }

                        TradeSkillItem item = new()
                        {
                            Name = (string)headerSkillItem["name"],
                            HeaderName = headerName,
                            Difficulty = difficulty,
                            ItemId = int.Parse(((string)headerSkillItem["link"]).Split(':')[1]),
                            Cooldown = cd
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

        var character = await db
            .Characters
            .Include(e => e.BnetProfessions)
                .ThenInclude(e => e.BnetProfessionItems)
                    .ThenInclude(e => e.BnetProfessionItemReagents)
            .FirstAsync(e => e.Id == characterId);

        db.RemoveRange(character.BnetProfessions);

        foreach (var tradeSkill in tradeSkills)
        {
            BnetProfession newProf = new()
            {
                BnetCharacterId = characterId,
                CurrentExp = tradeSkill.CurrentExp,
                MaxExp = tradeSkill.MaxExp,
                Name = tradeSkill.Name,
                SubSpecialisation = tradeSkill.SubSpecialisation,
                BnetProfessionItems = []
            };
            foreach (var tradeSkillItem in tradeSkill.Items)
            {
                var bnetProfItem = await itemService.InsertProfessionItem(tradeSkillItem);
                newProf.BnetProfessionItems.Add(bnetProfItem);
            }
            db.Add(newProf);
            await db.SaveChangesAsync();
        }

        foreach (var craftSkill in craftSkills)
        {
            BnetProfession newProf = new()
            {
                BnetCharacterId = characterId,
                CurrentExp = craftSkill.CurrentExp,
                MaxExp = craftSkill.MaxExp,
                Name = craftSkill.Name,
                BnetProfessionItems = []
            };
            foreach (var craftSkillItem in craftSkill.Items)
            {
                var bnetProfItem = await itemService.InsertProfessionItem(craftSkillItem);
                newProf.BnetProfessionItems.Add(bnetProfItem);
            }
            db.Add(newProf);
            await db.SaveChangesAsync();
        }

        return Ok();
    }
}
