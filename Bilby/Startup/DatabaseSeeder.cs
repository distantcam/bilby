using Bilby.Data;
using Bilby.Services;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Startup;

public class DatabaseSeeder(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        AddSeedData(
            scope.ServiceProvider.GetRequiredService<SettingsService>(),
            scope.ServiceProvider.GetRequiredService<IPasswordHasher>()
        );

        return Task.CompletedTask;
    }

    private void AddSeedData(
        SettingsService settings,
        IPasswordHasher passwordHasher)
    {
        var seedKey = "seedData";
        var seedVersion = "1";

        if (settings.TryGetValue(seedKey, out var version) && version == seedVersion)
            return;

        var password = configuration["Dev:Password"] ??
            throw new Exception("Dev Password must be set");
        var hashedPassword = passwordHasher.HashPassword(password);
        settings[SettingsService.PasswordKey] = hashedPassword;

        settings[SettingsService.ServerTitleKey] =
            configuration["Server:Title"] ?? "Bilby";
        settings[SettingsService.ShortDescriptionKey] =
            configuration["Server:ShortDescription"] ?? "The original Bilby server";
        settings[SettingsService.DescriptionKey] =
            configuration["Server:Description"] ?? "<p>The original Bilby server</p>";
        settings[SettingsService.EmailKey] =
            configuration["Server:Email"] ?? "";

        settings[SettingsService.UsernameKey] =
            configuration["User:Username"] ?? "bilby";
        settings[SettingsService.DisplayNameKey] =
            configuration["User:DisplayName"] ?? "bilby";
        settings[SettingsService.SummaryKey] =
            configuration["User:Summary"] ?? "<p>A small single user activitypub implementation.</p>";

        settings[seedKey] = seedVersion;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
