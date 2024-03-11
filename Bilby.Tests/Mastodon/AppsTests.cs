using System.Text.Json.Nodes;
using Bilby.Tests.Infrastructure;

namespace Bilby.Tests.Mastodon;

public class AppsTests(IntegrationTestWebAppFactory factory) : BaseEndpointTests(factory)
{
    [Fact]
    public async Task CreateApps()
    {
        await Verify(await this.PostAsForm("/api/v1/apps",
            ("client_name", nameof(CreateApps)),
            ("redirect_uris", "urn:ietf:wg:oauth:2.0:oob"),
            ("scopes", "read write"),
            ("website", "bilby.io")
        )).ScrubMember("id");
    }

    [Fact]
    public async Task CreateApp_Metatext2()
    {
        var json = @"
{
    ""scopes"": ""read write follow push"",
    ""redirect_uris"": ""metatext://oauth.callback"",
    ""website"": ""https://metabolist.org/metatext"",
    ""client_name"": ""Metatext""
}
";

        await Verify(await this.PostAsJson("/api/v1/apps", JsonNode.Parse(json)))
            .ScrubMember("id");
    }

    [Fact]
    public async Task CreateApp_IceCube()
    {
        await Verify(await this.Post("/api/v1/apps?client_name=IceCubesApp&redirect_uris=icecubesapp://&scopes=read%20write%20follow%20push&website=https://github.com/Dimillian/IceCubesApp"))
            .ScrubMember("id");
    }
}
