using Bilby.ActivityStream;
using Bilby.Data;
using Bilby.Middleware;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Users;

public class Inbox
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapPost("/inbox", Handle)
        .RequireAuthorization(SignatureAuthentication.Schema);

    private static async Task<IResult> Handle(
        Activity activity,
        ILogger<Inbox> logger,
        AppDbContext appDbContext,
        AccountLookupManager accountLookupManager
    )
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(activity.Type, "Follow"))
        {
            var actor = activity.Actor.AsString();
            var account = await accountLookupManager.GetAccount(actor);
            var follower = new Follower
            {
                FollowId = activity.Id?.AsString() ?? throw new Exception($"Follow ID is null"),
                Account = account ?? throw new Exception($"Could not get actor '{actor}'")
            };
            appDbContext.Add(follower);
            await appDbContext.SaveChangesAsync();
            logger.LogInformation("New follower added! {follower}", actor);
            return Accepted((string?)null);
        }

        if (StringComparer.OrdinalIgnoreCase.Equals(activity.Type, "Undo"))
        {
            if (activity["object"] is Activity undoActivity &&
                StringComparer.OrdinalIgnoreCase.Equals(undoActivity.Type, "Follow") &&
                undoActivity.Actor.IsString)
            {
                var follows = await appDbContext.Followers
                    .Where(f => f.FollowId == undoActivity.Id.AsString() &&
                        f.Account.ActorUrl == undoActivity.Actor.AsString())
                    .ToListAsync()
                    .ConfigureAwait(false);
                if (follows.Count != 0)
                {
                    appDbContext.Followers.RemoveRange(follows);
                    await appDbContext.SaveChangesAsync();
                }
                logger.LogInformation("Lost a follower {follower}", undoActivity.Actor.AsString());
                return Accepted((string?)null);
            }
        }

        return BadRequest();
    }
}
