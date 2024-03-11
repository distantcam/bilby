using Bilby.Tests.Infrastructure;

public class WellKnownTests(IntegrationTestWebAppFactory factory) : BaseEndpointTests(factory)
{
    [Fact]
    public async Task HostMeta() => await Verify(await this.Get("/.well-known/host-meta"));

    [Fact]
    public async Task NodeInfo() => await Verify(await this.Get("/.well-known/nodeinfo"));

    [Fact]
    public async Task NodeInfo20() => await Verify(await this.Get("/nodeinfo/2.0.json"));

    [Fact]
    public async Task Webfinger() => await Verify(await this.Get($"/.well-known/webfinger?resource=acct:bilby@localhost"));

    [Fact]
    public async Task MissingWebfinger() => await Verify(await this.Get($"/.well-known/webfinger?resource=acct:{Guid.NewGuid()}@localhost"))
        .ScrubInlineGuids();
}
