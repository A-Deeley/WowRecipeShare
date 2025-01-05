namespace recipe_share_api.Login;

public class PatchBnetUserRequest
{
    public int Id { get; set; }
    public int? PreferredRealmId { get; set; }
    public int? PreferredAccountId { get; set; }
}
