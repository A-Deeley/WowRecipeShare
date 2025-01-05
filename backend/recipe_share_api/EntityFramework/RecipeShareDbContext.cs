using Microsoft.EntityFrameworkCore;

namespace recipe_share_api.EntityFramework;

public class RecipeShareDbContext : DbContext
{
    public RecipeShareDbContext(DbContextOptions<RecipeShareDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BnetUser>()
            .HasMany(e => e.Accounts)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);

        //modelBuilder.Entity<BnetUser>()
        //    .HasOne(e => e.PreferredAccount)
        //    .WithOne(e => e.User);

        //modelBuilder.Entity<BnetUser>()
        //    .HasOne(e => e.PreferredRealm)
        //    .WithMany(e => e.Users)
        //    .HasForeignKey(e => e.PreferredRealmId);

        //modelBuilder.Entity<BnetRealm>()
        //    .HasMany(e => e.Characters)
        //    .WithOne(e => e.Realm)
        //    .HasForeignKey(e => e.RealmId);

        //modelBuilder.Entity<BnetRealm>()
        //    .HasMany(e => e.Users)
        //    .WithOne(e => e.PreferredRealm)
        //    .HasForeignKey(e => e.PreferredRealmId);

        //modelBuilder.Entity<BnetUserAccount>()
        //    .HasMany(e => e.BnetCharacters)
        //    .WithOne(e => e.Account)
        //    .HasForeignKey(e => e.AccountId);

        //modelBuilder.Entity<BnetUserAccount>()
        //    .HasOne(e => e.User)
        //    .WithMany(e => e.Accounts)
        //    .HasForeignKey(e => e.UserId);
    }

    public DbSet<BnetUser> BnetUsers { get; set; }
}
