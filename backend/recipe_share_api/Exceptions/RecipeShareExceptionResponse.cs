using recipe_share_api.Exceptions.AddonExceptions;

namespace recipe_share_api.Exceptions;

public class RecipeShareExceptionResponse
{
    public string Message { get; set; }

    public RecipeShareExceptionResponse(RecipeShareException ex)
    {
        if (ex is InvalidVersionException ivEx)
            Message = $"{ex.Message} (You have: {ivEx.Version} | Requires: {ivEx.ExpectedVersion})";
        else
            Message = ex.Message;
    }
}
