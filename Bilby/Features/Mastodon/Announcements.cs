using EndpointGenerator;

namespace Bilby.Features.Mastodon;

public static class Announcements
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/announcements", Handle)
        .RequireAuthorization();

    private static IResult Handle()
    {
        return Ok<IEnumerable<Models.Mastodon.Announcement>>([]);
    }
}
