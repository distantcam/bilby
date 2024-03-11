using Bilby.Data;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.Azurite;
using Testcontainers.PostgreSql;

namespace Bilby.Tests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<AppJsonSerializerContext>, IAsyncLifetime
{
    public AppDbContext ArrangeDbContext { get; private set; } = default!;
    public AppDbContext AssertDbContext { get; private set; } = default!;

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    private readonly AzuriteContainer _blobStoreContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddLogging(builder => builder.ClearProviders().AddConsole().AddDebug());
            services.AddSingleton<IPolicyEvaluator, TestPolicyEvaluator>();
        });
    }

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _blobStoreContainer.StartAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings__Db", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings__BlobStorage", _blobStoreContainer.GetConnectionString());

        var dbFactory = Services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        ArrangeDbContext = dbFactory.CreateDbContext();
        AssertDbContext = dbFactory.CreateDbContext();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        ArrangeDbContext?.Dispose();
        AssertDbContext?.Dispose();

        await _dbContainer.StopAsync();
        await _blobStoreContainer.StopAsync();
    }
}
