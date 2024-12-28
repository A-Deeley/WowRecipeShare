using System.Text.Json.Serialization;

namespace recipe_share_api.Characters;
public class SkillItem
{
    public string Name { get; set; } = string.Empty;
    public int ItemId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<Difficulty>))]
    public Difficulty Difficulty { get; set; }
    public List<SkillReagent> Reagents { get; set; } = [];
}
