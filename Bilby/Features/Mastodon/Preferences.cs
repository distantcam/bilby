using EndpointGenerator;

namespace Bilby.Features.Mastodon;

public static class Preferences
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/preferences", Handle)
        .RequireScopeAuthorization("read:accounts");

    private static IResult Handle(HttpContext httpContext)
    {
        return Ok(new Dictionary<string, object>() {
            { "posting:default:visibility", "public" },
            { "posting:default:sensitive", false },
            { "posting:default:language", "en" },
            { "reading:expand:media", "default" },
            { "reading:expand:spoilers", false },
        });
    }
}
