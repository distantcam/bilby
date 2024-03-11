using Bilby.ActivityStream;
using Bilby.Data;
using Bilby.Models.ActivityStream;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Users;

public static class Followers
{
    private static readonly int s_pagesize = 10;

    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/followers", Handle);

    private static async Task<IResult> Handle(
        int? page,
        UriGenerator uriGenerator,
        AppDbContext appDbContext
    )
    {
        var followerCount = await appDbContext.Followers.CountAsync();
        var lastPage = Math.Ceiling((double)followerCount / s_pagesize);

        if (page.HasValue)
        {
            var pageOfFollowers = await appDbContext.Followers
                .Skip((page.Value - 1) * s_pagesize)
                .Take(s_pagesize)
                .Select(f => f.Account.ActorUrl)
                .ToArrayAsync();
            var followersPage = new OrderedCollectionPage
            {
                Id = uriGenerator.GetCurrentUri(),
                TotalItems = followerCount,
                PartOf = uriGenerator.GetUri($"/followers"),
                Items = [.. pageOfFollowers]
            };

            if (page > 1)
                followersPage.Prev =
                    uriGenerator.GetUri($"/followers?page={page - 1}");
            if (page < lastPage)
                followersPage.Next =
                    uriGenerator.GetUri($"/followers?page={page + 1}");

            return ActivityStreamObject(followersPage);
        }

        var followers = new OrderedCollection
        {
            Id = uriGenerator.GetCurrentUri(),
            TotalItems = followerCount,
            First = uriGenerator.GetUri($"/followers?page=1")
        };
        return ActivityStreamObject(followers);
    }
}
