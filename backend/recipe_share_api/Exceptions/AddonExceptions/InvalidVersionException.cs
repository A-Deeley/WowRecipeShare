namespace recipe_share_api.Exceptions.AddonExceptions;

public class InvalidVersionException(string version, string expectedVersion) : RecipeShareException("Addon version mismatch. Update the addon and try again.")
{
    public string Version { get; } = version;
    public string ExpectedVersion { get; } = expectedVersion;
}
