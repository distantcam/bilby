using System.Text.Json.Serialization;
using Bilby.Data;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Micropub;

public static class Query
{
    [EndpointGroupBuilder]
    public static void FormPost(RouteGroupBuilder builder) => builder
        .MapGet("/micropub", Handle)
        .RequireAuthorization();

    private static async Task<IResult> Handle(
        string q,
        string? url,
        AppDbContext appDbContext,
        UriGenerator uriGenerator
    )
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(q, "config"))
            return Ok(new ConfigResponse());

        if (StringComparer.OrdinalIgnoreCase.Equals(q, "source"))
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest();

            var postId = uriGenerator.GetPostId(url);
            var post = await appDbContext.Posts
                .FirstAsync(e => e.Id == postId);
        }

        return BadRequest();
    }

    public record struct ConfigResponse(
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("media-endpoint")]
        string? MediaEndpoint
    );
}
