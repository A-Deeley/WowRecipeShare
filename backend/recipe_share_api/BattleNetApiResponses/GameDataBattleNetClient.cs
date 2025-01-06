using recipe_share_api.EntityFramework;
using System.Text.Json;

namespace recipe_share_api.BattleNetApiResponses;

public class GameDataBattleNetClient(HttpClient client)
{
    readonly string baseUrl = "https://us.api.blizzard.com";
    readonly string _dynamicParams = "namespace=dynamic-classic1x-us&locale=en_US";
    readonly string _staticParams = "namespace=static-classic1x-us&locale=en_US";

    public async Task<IEnumerable<Realm>> RealmsIndex()
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer");

        var response = await client.GetAsync($"{baseUrl}/data/wow/realm/index?{_dynamicParams}");

        GetRealmIndexResponse val = await JsonSerializer.DeserializeAsync<GetRealmIndexResponse>(response.Content.ReadAsStream()) ?? throw new InvalidOperationException("Error decoding realm response from blizzard server.");

        return val.realms;
    }

    public async Task<ItemResponse?> Item(int itemId)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer");

        var response = await client.GetAsync($"{baseUrl}/data/wow/item/{itemId}?{_staticParams}");

        var itemResponse = await JsonSerializer.DeserializeAsync<ItemResponse>(response.Content.ReadAsStream());

        return itemResponse;
    }
}


public class ItemResponse
{
    public int id { get; set; }
    public string name { get; set; }
    public Quality quality { get; set; }
    public int level { get; set; }
    public int required_level { get; set; }
    public ItemClass item_class { get; set; }
    public ItemSubclass item_subclass { get; set; }

    public BnetItem MapToBnetItem()
    {
        var bnetItem = new BnetItem()
        {
            Id = id,
            ItemClassId = item_class.id,
            ItemClassName = item_class.name,
            ItemSubclassId = item_subclass.id,
            ItemSubclassName = item_subclass.name,
            ItemLevel = level,
            Name = name,
            Quality = quality.name,
            RequiredLevel = required_level
        };

        return bnetItem;
    }
}

public class Quality
{
    public string type { get; set; }
    public string name { get; set; }
}


public class ItemClass
{
    public string name { get; set; }
    public int id { get; set; }
}

public class ItemSubclass
{
    public string name { get; set; }
    public int id { get; set; }
}