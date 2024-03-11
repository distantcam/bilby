using Bilby.ActivityStream;
using Bilby.Data;
using Bilby.Models.ActivityStream;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.AspNetCore.Mvc;

namespace Bilby.Features.Users;

public static class Following
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/following", Handle);

    private static IResult Handle(
        [FromQuery] uint? page,
        UriGenerator uriGenerator,
        AppDbContext appDbContext
    )
    {
        if (page.HasValue)
        {
            return StatusCode(501);
        }

        var following = new OrderedCollection
        {
            Id = uriGenerator.GetCurrentUri(),
            TotalItems = 0,
            First = uriGenerator.GetUri($"/following?page=1")
        };
        return ActivityStreamObject(following);
    }
}
