using System.ComponentModel.DataAnnotations;

namespace recipe_share_api.Login;

public class PostLoginRequest
{
    [Required]
    public string Code { get; set; } = null!;
}
