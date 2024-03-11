using EndpointGenerator;

namespace Bilby.Features.Mastodon;

public static class Filters
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/filters", Handle)
        .RequireAuthorization();

    private static IResult Handle()
    {
        return Ok<IEnumerable<Models.Mastodon.Filter>>([]);
    }
}
