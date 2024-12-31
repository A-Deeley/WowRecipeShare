using System.Text.Json.Serialization;

namespace recipe_share_api.Characters;

public class ItemCooldown
{
    [JsonIgnore]
    public Int64 Current { get; set; }

    [JsonIgnore]
    public double Delta { get; set; }

    public DateTime CooldownEnd
    {
        get
        {
            DateTime current = DateTime.UnixEpoch.AddSeconds(Current).ToLocalTime();
            TimeSpan delta = TimeSpan.FromSeconds(Delta);
            return current + delta;
        }
    }

    public TimeSpan CooldownDuration
    {
        get
        {
            return TimeSpan.FromSeconds(Delta);
        }
    }
}
