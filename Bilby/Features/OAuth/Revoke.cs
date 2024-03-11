using System.Text.Json.Serialization;

namespace Bilby.Features.OAuth;

public static class Revoke
{
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapPost("/oauth/revoke", Handle);

    public record RevokeRequest(
        [property: JsonPropertyName("client_id")] string ClientId,
        [property: JsonPropertyName("client_secret")] string ClientSecret,
        [property: JsonPropertyName("token")] string Token
    );

    public static IResult Handle(
        RevokeRequest revokeRequest
    )
    {
        return StatusCode(501);
    }
}
