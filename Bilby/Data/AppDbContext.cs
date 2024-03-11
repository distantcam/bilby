using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bilby.Data;

public class AppDbContext(IConfiguration configuration, DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<StoredSetting> StoredSettings { get; set; }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Follower> Followers { get; set; }

    public DbSet<Application> Applications { get; set; }

    public DbSet<Post> Posts { get; set; }

    // ------------------------------------------------------------------------

    private readonly string _connectionString =
        configuration.GetConnectionString("Db") ??
        throw new Exception("Db connection string is not set");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            convertTo => DateTime.SpecifyKind(convertTo, DateTimeKind.Utc),
            convertFrom => convertFrom
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(utcConverter);
                }
            }
        }
    }
}
