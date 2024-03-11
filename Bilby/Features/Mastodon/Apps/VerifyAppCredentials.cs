using Bilby.Data;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Mastodon.Apps;

public static class VerifyAppCredentials
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/apps/verify_credentials", Handle)
        .RequireScopeAuthorization("read");

    private static async Task<IResult> Handle(
        HttpContext httpContext,
        AppDbContext appDbContext,
        CancellationToken cancellationToken
    )
    {
        var clientId = httpContext.User.FindFirst("clientId")?.Value;
        if (clientId is null)
            return Unauthorized();

        var app = await appDbContext.Applications
            .FirstOrDefaultAsync(a => a.ClientId == clientId, cancellationToken);
        if (app is null)
            return Unauthorized();

        return Ok(new Models.Mastodon.Application
        {
            Name = app.ClientName,
            Website = app.Website,
        });
    }
}
