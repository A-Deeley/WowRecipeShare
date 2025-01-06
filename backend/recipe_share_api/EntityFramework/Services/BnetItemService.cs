using Microsoft.EntityFrameworkCore;
using recipe_share_api.BattleNetApiResponses;
using recipe_share_api.Characters;

namespace recipe_share_api.EntityFramework.Services;

public class BnetItemService(RecipeShareDbContext db, GameDataBattleNetClient gameDataClient)
{
    public async Task<BnetProfessionItem> InsertProfessionItem(SkillItem tradeItem)
    {
        var existingItem = await db
            .Items
            .FirstOrDefaultAsync(i => i.Id == tradeItem.ItemId);

        if (existingItem is null)
            await GetAndSaveItem(tradeItem.ItemId);

        List<BnetProfessionItemReagent> reagents = [];

        foreach (var reagent in tradeItem.Reagents)
        {
            existingItem = await db
                .Items
                .FirstOrDefaultAsync(i => i.Id == reagent.ItemId);

            existingItem ??= await GetAndSaveItem(reagent.ItemId);

            BnetProfessionItemReagent bnetReagent = new()
            {
                BnetItemId = reagent.ItemId,
                Count = reagent.Count,
                BnetProfessionItemId = tradeItem.ItemId,
                Name = reagent.Name ?? existingItem.Name
            };
            reagents.Add(bnetReagent);
        }

        BnetProfessionItem professionItem = new()
        {
            Difficulty = Enum.GetName(tradeItem.Difficulty)!,
            Name = tradeItem.Name,
            BnetItemId = tradeItem.ItemId,
            BnetProfessionItemReagents = reagents
        };

        db.Add(professionItem);
        await db.SaveChangesAsync();

        return professionItem;
    }

    public async Task<BnetProfessionItem> InsertProfessionItem(TradeSkillItem tradeItem)
    {
        var existingItem = await db
            .Items
            .FirstOrDefaultAsync(i => i.Id == tradeItem.ItemId);

        if (existingItem is null)
            await GetAndSaveItem(tradeItem.ItemId);

        List<BnetProfessionItemReagent> reagents = [];

        foreach (var reagent in tradeItem.Reagents)
        {
            existingItem = await db
                .Items
                .FirstOrDefaultAsync(i => i.Id ==  reagent.ItemId);

            existingItem ??= await GetAndSaveItem(reagent.ItemId);

            BnetProfessionItemReagent bnetReagent = new()
            {
                BnetItemId = reagent.ItemId,
                Count = reagent.Count,
                BnetProfessionItemId = tradeItem.ItemId,
                Name = reagent.Name ?? existingItem.Name
            };
            reagents.Add(bnetReagent);
        }

        BnetProfessionItem professionItem = new()
        {
            Current = tradeItem.Cooldown?.Current,
            Delta = tradeItem.Cooldown?.Delta,
            Difficulty = Enum.GetName(tradeItem.Difficulty)!,
            HeaderName = tradeItem.HeaderName,
            Name = tradeItem.Name,
            BnetItemId = tradeItem.ItemId,
            BnetProfessionItemReagents = reagents
        };

        db.Add(professionItem);
        await db.SaveChangesAsync();

        return professionItem;
    }

    async Task<BnetItem> GetAndSaveItem(int itemId)
    {
        var getItemResponse = await gameDataClient.Item(itemId) ?? throw new InvalidOperationException("Item not found!");

        var item = getItemResponse.MapToBnetItem();
        db.Add(item);
        await db.SaveChangesAsync();

        return item;
    }
}
