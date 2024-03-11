using System.Text.Json.Nodes;
using Bilby.Tests.Infrastructure;

namespace Bilby.Tests.Mastodon;

public class OAuth(IntegrationTestWebAppFactory factory) : BaseEndpointTests(factory)
{
    [Fact]
    public async Task FullLoginFlow()
    {
        var appRegistrationResponse = await this.PostAsForm("/api/v1/apps",
            ("client_name", nameof(FullLoginFlow)),
            ("redirect_uris", "urn:ietf:wg:oauth:2.0:oob"),
            ("scopes", "read write"),
            ("website", "bilby.io")
        );

        var appRegistration = JsonNode.Parse(await appRegistrationResponse.Content.ReadAsStringAsync()) ??
            throw new NullReferenceException();

        var query = ToQuery(
            ("response_type", "code"),
            ("client_id", appRegistration["client_id"]!.GetValue<string>()),
            ("redirect_uri", appRegistration["redirect_uri"]!.GetValue<string>()),
            ("scope", "read write")
        );

        var authorizeResponse = await this.UsingTestAuth().Get($"/oauth/authorize?{query}");

        var code = (await authorizeResponse.Content.ReadAsStringAsync()).Trim('\"');

        var tokenResponse = await this.UsingTestAuth().PostAsForm("/oauth/token",
            ("grant_type", "authorization_code"),
            ("code", code),
            ("client_id", appRegistration["client_id"]!.GetValue<string>()),
            ("redirect_uri", appRegistration["redirect_uri"]!.GetValue<string>())
        );

        var token = (await tokenResponse.Content.ReadAsStringAsync()).Trim('\"');

        await Verify(this.WithBearerToken(token).Get("/api/v1/apps/verify_credentials"))
            .ScrubMember("Authorization");
    }

    private static string ToQuery(params (string Key, string Value)[] args) =>
        string.Join("&", args.Select(kv => kv.Key + "=" + kv.Value));
}
