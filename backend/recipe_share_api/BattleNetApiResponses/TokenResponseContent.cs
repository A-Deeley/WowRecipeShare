﻿namespace recipe_share_api.BattleNetApiResponses;

public class TokenResponseContent
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public string scope { get; set; }
    public string sub { get; set; }
    public string id_token { get; set; }
}
