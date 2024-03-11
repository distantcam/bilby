using Bilby.Tests.Infrastructure;

namespace Bilby.Tests.ActivityPub;

public class UserTests(IntegrationTestWebAppFactory factory) : BaseEndpointTests(factory)
{
    [Fact]
    public async Task Users() => await Verify(await this.WithASHeader().Get("/@bilby"));

    [Fact]
    public async Task Outbox() => await Verify(await this.WithASHeader().Get("/outbox"));

    [Fact]
    public async Task Followers() => await Verify(await this.WithASHeader().Get("/followers"));

    [Fact]
    public async Task Following() => await Verify(await this.WithASHeader().Get("/following"));
}
