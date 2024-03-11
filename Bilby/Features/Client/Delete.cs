using Bilby.Data;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Client;

public static class Delete
{
    [EndpointGroupBuilder]
    public static void MapEndpoints(RouteGroupBuilder builder)
    {
        builder
            .MapDelete("/api/post/{id}", Handle)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        string id,
        AppDbContext appDbContext
    )
    {
        var postId = long.Parse(id);
        var post = await appDbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post is null)
            return NotFound();

        post.IsDeleted = true;
        await appDbContext.SaveChangesAsync();

        return NoContent();
    }
}
