using System.Runtime.Serialization;

namespace recipe_share_api.Exceptions;

public abstract class RecipeShareException : Exception
{
    protected RecipeShareException()
    {
    }

    protected RecipeShareException(string? message) : base(message)
    {
    }

    protected RecipeShareException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
