using Bilby.Models;
using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.WebFinger;

public static class WebFinger
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/.well-known/webfinger", Handler)
        .Produces(404)
        .Produces<UserDetails>();

    public record struct UserDetails(string Subject, IEnumerable<string>? Aliases, IEnumerable<WebFingerLink>? Links);
    public record struct WebFingerLink(string Rel, string Href, string Type);

    private static IResult Handler(
        string resource,
        UriGenerator uriGenerator,
        SettingsService settingsService
    )
    {
        if (!AcctUri.TryParse(resource, out var acct) || acct == null ||
            !StringComparer.OrdinalIgnoreCase.Equals(acct.User, settingsService[SettingsService.UsernameKey]))
            return NotFound();

        var userPage = uriGenerator.GetProfileUri(settingsService[SettingsService.UsernameKey]);

        var userDetails = new UserDetails(
            acct.ToString(),
            [userPage],
            [
                new("self", userPage, MimeTypes.ActivityJson),
                new("http://webfinger.net/rel/avatar", uriGenerator.GetUri("/images/avatar.png"), "image/jpeg")
            ]
        );

        return Ok(userDetails);
    }
}
