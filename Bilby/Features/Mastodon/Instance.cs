using System.Reflection;
using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.Mastodon;

public class Instance
{
    [EndpointGroupBuilder]
    public static void MapEndpoints(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/instance", Handle);

    private static IResult Handle(
        UriGenerator uriGenerator,
        SettingsService settingsService
    )
    {
        return Ok(new Models.Mastodon.InstanceV1
        {
            Uri = uriGenerator.GetHostName(),
            Title = settingsService[SettingsService.ServerTitleKey],
            ShortDescription = settingsService[SettingsService.ShortDescriptionKey],
            Description = settingsService[SettingsService.DescriptionKey],
            Email = settingsService[SettingsService.EmailKey],
            Version = Assembly.GetCallingAssembly().GetName().Version?.ToString(3) ?? "0.0.0",

            Urls = default!,
            Stats = new() { UserCount = 1, StatusCount = 0, DomainCount = 1 },
            Thumbnail = default!,
            Languages = ["en"],

            Configuration = default!,
            ContactAccount = default!,
            Rules = default!
        });
    }
}
