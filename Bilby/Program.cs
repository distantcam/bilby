using Bilby.Data;
using Bilby.Middleware;
using Bilby.Models;
using Bilby.Startup;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.WebHost.UseKestrelHttpsConfiguration();

builder.Services
    .AddDbContextFactory<AppDbContext>()
    .AddDbContext<AppDbContext>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.Configure<JwtConfig>(config.GetSection(JwtConfig.SectionName));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCors();
builder.Services.AddAntiforgery();
builder.Services.AddHttpContextAccessor();
builder.Services.AddBilbyHttpClient();
builder.Services.AddBilbyAuth(
    config["Jwt:Issuer"] ?? throw new Exception("Config Jwt:Issuer not set"),
    config["Jwt:Audience"] ?? throw new Exception("Config Jwt:Audience not set"),
    config["Jwt:Key"] ?? throw new Exception("Config Jwt:Key not set")
);
builder.Services.AddBilby();

#if DEBUG
builder.Services.AddHostedService<DatabaseSeeder>();
builder.Services.AddHostedService<StorageSeeder>();
#endif

// ----------------------------------------------------------------------------

var app = builder.Build();

app.UseServerHeader();
app.UseLinkAuthHeaders();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(cors => cors.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapBilbyEndpoints();

app.Run();
