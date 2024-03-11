using Bilby.Data;

namespace Bilby.Tests.Infrastructure;

public class BaseEndpointTests(IntegrationTestWebAppFactory factory) :
    IClassFixture<IntegrationTestWebAppFactory>, IHttpClient
{
    public AppDbContext ArrangeDbContext => factory.ArrangeDbContext;
    public AppDbContext AssertDbContext => factory.AssertDbContext;

    Func<HttpClient> IHttpClient.ClientFunc { get; } = () => factory.CreateDefaultClient();
}
