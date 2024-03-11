using EndpointGenerator;

namespace Bilby.Features.Mastodon;

public static class CustomEmojis
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/custom_emojis", Handle);

    private static IResult Handle()
    {
        return Ok<IEnumerable<Models.Mastodon.CustomEmoji>>(null);
    }
}
