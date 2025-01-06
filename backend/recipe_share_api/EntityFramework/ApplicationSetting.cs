namespace recipe_share_api.EntityFramework;

public class ApplicationSetting : Entity
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;

    public const string AddonVersion = "AddonVersion";
}
