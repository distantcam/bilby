using Bilby.ActivityStream;
using Bilby.Models;
using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.Users;

public static class UserEndpoint
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder)
    {
        builder
            .MapGet("/@{username}", Handler)
            .RequireAcceptActivityStream()
            .Produces(404)
            .Produces<Person>(200, MimeTypes.ActivityJson);
    }

    private static IResult Handler(
        string username,
        UriGenerator uriGenerator,
        SettingsService settingsService
    )
    {
        if (!StringComparer.OrdinalIgnoreCase.Equals(username, settingsService[SettingsService.UsernameKey]))
            return NotFound();

        Person person = new()
        {
            Context = ["https://www.w3.org/ns/activitystreams", "https://w3id.org/security/v1"],
            Id = uriGenerator.GetCurrentUri(),
            Name = settingsService[SettingsService.DisplayNameKey],
            PreferredUsername = settingsService[SettingsService.UsernameKey],
            Summary = settingsService[SettingsService.SummaryKey],

            Inbox = uriGenerator.GetUri($"/inbox"),
            Outbox = uriGenerator.GetUri($"/outbox"),
            Followers = uriGenerator.GetUri($"/followers"),
            Following = uriGenerator.GetUri($"/following"),

            Endpoints = new()
            {
                SharedInbox = uriGenerator.GetUri($"/inbox")
            }
        };
        person.Add("manuallyApprovesFollowers", false);
        person.Add("discoverable", true);
        person.Add("icon", uriGenerator.GetUri("/images/avatar.png"));

        return ActivityStreamObject(person);
    }
}
