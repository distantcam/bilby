using System.Reflection;
using System.Text.Json.Serialization;
using Bilby.Data;
using EndpointGenerator;
using Microsoft.EntityFrameworkCore;

namespace Bilby.Features.Mastodon.Apps;

public static class AppsEndpoint
{
    [EndpointGroupBuilder]
    public static void MapEndpoints(RouteGroupBuilder builder) => builder
        .MapPost("/api/v1/apps", Handle);

    public record AppsRequest(
        [property: JsonPropertyName("client_name")] string ClientName,
        [property: JsonPropertyName("redirect_uris")] string RedirectUris,
        [property: JsonPropertyName("scopes")] string Scopes,
        [property: JsonPropertyName("website")] string? Website
    )
    {
        public static ValueTask<AppsRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
        {
            var req = httpContext.Request;

            if (req.HasJsonContentType())
            {
                return req.ReadFromJsonAsync<AppsRequest>();
            }

            if (req.HasFormContentType)
            {
                return ValueTask.FromResult<AppsRequest?>(new(
                    req.Form["client_name"]!,
                    req.Form["redirect_uris"]!,
                    req.Form["scopes"]!,
                    req.Form["website"]
                ));
            }

            return ValueTask.FromResult<AppsRequest?>(new(
                req.Query["client_name"]!,
                req.Query["redirect_uris"]!,
                req.Query["scopes"]!,
                req.Query["website"]
            ));
        }
    }

    private static async Task<IResult> Handle(
        AppsRequest request,
        AppDbContext appDbContext
    )
    {
        var app = await appDbContext.Applications
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ClientName == request.ClientName);
        if (app is null)
        {
            app = new()
            {
                ClientName = request.ClientName,
                Scopes = request.Scopes.Split(' ').ToList(),
                Website = request.Website,

                ClientId = Guid.NewGuid().ToString("N"),
                ClientSecret = Guid.NewGuid().ToString("N")
            };
            appDbContext.Applications.Add(app);
            await appDbContext.SaveChangesAsync();
        }

        return Ok(new AppWithExtras()
        {
            Id = app.Id.ToString(),
            Name = request.ClientName,
            Website = request.Website,
            RedirectUri = request.RedirectUris,
            ClientId = app.ClientId,
            ClientSecret = app.ClientSecret
        });
    }

    public record AppWithExtras : Models.Mastodon.Application
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("redirect_uri")]
        public required string RedirectUri { get; set; }
    }
}
