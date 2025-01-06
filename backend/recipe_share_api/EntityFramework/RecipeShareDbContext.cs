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
            .HasMany(e => e.BnetUserAccounts)
            .WithOne(e => e.BnetUser)
            .HasForeignKey(e => e.BnetUserId);

        //modelBuilder.Entity<BnetUser>()
        //    .HasOne(e => e.PreferredAccount)
        //    .WithOne(e => e.BnetUser)
        //    .HasForeignKey<BnetUser>(e => e.PreferredAccountId);

        //modelBuilder.Entity<BnetUser>()
        //    .HasOne(e => e.PreferredRealm)
        //    .WithMany(e => e.BnetUsers)
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

    public DbSet<BnetUser> Users { get; set; }
    public DbSet<BnetRealm> Realms { get; set; }
    public DbSet<BnetItem> Items { get; set; }
    public DbSet<BnetCharacter> Characters { get; set; }
    public DbSet<BnetProfession> Professions { get; set; }
    public DbSet<BnetProfessionItem> ProfessionItems { get; set; }
    public DbSet<BnetProfessionItemReagent> ProfessionItemReagents { get; set; }
    public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
}
