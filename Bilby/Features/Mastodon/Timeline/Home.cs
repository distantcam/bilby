using EndpointGenerator;
using Microsoft.AspNetCore.Mvc;

namespace Bilby.Features.Mastodon.Timeline;

public static class Home
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/timelines/home", Handle)
        .RequireAuthorization();

    private static IResult Handle(
        [FromQuery(Name = "max_id")] string? maxId,
        [FromQuery(Name = "since_id")] string? sinceId,
        [FromQuery(Name = "min_id")] string? minId,
        [FromQuery(Name = "limit")] string? limit = "20"
    )
    {
        return Ok<IEnumerable<Models.Mastodon.Status>>([]);
    }
}
