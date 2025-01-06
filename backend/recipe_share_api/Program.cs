
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using recipe_share_api.BattleNetApiResponses;
using recipe_share_api.EntityFramework;
using recipe_share_api.EntityFramework.Services;
using recipe_share_api.Sessions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

#if DEBUG
builder.Services.AddDbContext<RecipeShareDbContext>(options => options
    .UseInMemoryDatabase("recipeShare")
    .UseSeeding((ctx, _) =>
    {
        var testAddonVersion = ctx.Set<ApplicationSetting>().FirstOrDefault(e => e.Key == ApplicationSetting.AddonVersion);
        if (testAddonVersion is null)
        {
            ctx.Set<ApplicationSetting>().Add(new() { Key =  ApplicationSetting.AddonVersion, Value = "0.1.9" });
            ctx.SaveChanges();
        }
    })
    .UseAsyncSeeding(async (ctx, _, cancellationToken) =>
    {
        var testAddonVersion = await ctx.Set<ApplicationSetting>().FirstOrDefaultAsync(e => e.Key == ApplicationSetting.AddonVersion, cancellationToken);
        if (testAddonVersion is null)
        {
            ctx.Set<ApplicationSetting>().Add(new() { Key = ApplicationSetting.AddonVersion, Value = "0.1.9" });
            await ctx.SaveChangesAsync(cancellationToken);
        }
    }));
#else
var cString = configuration.GetConnectionString("MySql");
var serverVersion = ServerVersion.AutoDetect(cString);
builder.Services.AddDbContext<RecipeShareDbContext>(options => options.UseMySql(cString, serverVersion));
#endif

builder.Services.AddControllersWithViews().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISessionState, SessionState>();
builder.Services.AddScoped<AuthenticationMiddleware>();
builder.Services.AddScoped<BnetItemService>();
builder.Services.AddHttpClient<GameDataBattleNetClient>().AddHttpMessageHandler<AuthenticationMiddleware>();
builder.Services.AddHttpClient<ProfileBattleNetClient>();
builder.Services.Configure<recipe_share_api.Options.OpenIdConnectOptions>(builder.Configuration.GetSection(recipe_share_api.Options.OpenIdConnectOptions.Key));
builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173", "https://recipeshare.kuronai.dev", "http://localhost:4173", "https://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseCors("Cors");

app.MapControllers();

// Perform realm lookup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RecipeShareDbContext>();
    dbContext.Database.EnsureCreated();
    var gameDataClient = scope.ServiceProvider.GetRequiredService<GameDataBattleNetClient>();
    var getRealmsResponse = await gameDataClient.RealmsIndex();
    foreach (var realm in getRealmsResponse)
    {
        if (dbContext.Realms.Any(r => r.Id == realm.id)) continue;
        BnetRealm dbRealm = new()
        {
            Id = realm.id,
            Name = realm.name,
            Slug = realm.slug
        };
        dbContext.Add(dbRealm);
    }

    await dbContext.SaveChangesAsync();
}

app.Run();
