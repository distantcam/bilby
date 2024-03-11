using Bilby.ActivityStream;
using Bilby.Models.ActivityStream;
using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.Users;

public static class Outbox
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/outbox", Handler);

    private static IResult Handler(
        bool? page,
        UriGenerator uriGenerator)
    {
        if (page.HasValue)
        {
            return StatusCode(501);
        }

        var postCollection = new OrderedCollection
        {
            Id = uriGenerator.GetCurrentUri(),
            TotalItems = 0,
            First = uriGenerator.GetUri("/outbox?page=true"),
            Last = uriGenerator.GetUri("/outbox?min_id=0&page=true")
        };

        return ActivityStreamObject(postCollection);
    }
}
