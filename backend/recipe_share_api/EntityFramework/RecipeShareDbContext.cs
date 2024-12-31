using Microsoft.EntityFrameworkCore;

namespace recipe_share_api.EntityFramework;

public class RecipeShareDbContext : DbContext
{
    public RecipeShareDbContext(DbContextOptions<RecipeShareDbContext> options) : base(options)
    {
        
    }


    public DbSet<BnetUser> BnetUsers { get; set; }
}
