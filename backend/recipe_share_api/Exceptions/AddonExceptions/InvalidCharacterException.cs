namespace recipe_share_api.Exceptions.AddonExceptions;

public class InvalidCharacterException() : RecipeShareException("The character being modified does not match the one found in the file. Choose the corresponding file and try again.")
{
}
