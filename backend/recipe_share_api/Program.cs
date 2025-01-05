
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using recipe_share_api.BattleNetApiResponses;
using recipe_share_api.EntityFramework;
using recipe_share_api.Sessions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

#if DEBUG
    builder.Services.AddDbContext<RecipeShareDbContext>(options => options.UseInMemoryDatabase("recipeShare"));
#else
    var cString = configuration.GetConnectionString("MySql");
    var serverVersion = ServerVersion.AutoDetect(cString);
    builder.Services.AddDbContext<RecipeShareDbContext>(options => options.UseMySql(cString, serverVersion));
#endif
builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISessionState, SessionState>();
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

app.Run();
