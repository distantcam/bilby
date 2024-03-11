using EndpointGenerator;

namespace Bilby.Features.Mastodon;

public class Markers
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/markers", Handle)
        .RequireAuthorization();

    private static IResult Handle()
    {
        return Ok<Models.Mastodon.Marker>(null);
    }
}
