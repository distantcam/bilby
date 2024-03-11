using Bilby.Data;
using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.Mastodon.Accounts;

public static class VerifyAccountCredentials
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/api/v1/accounts/verify_credentials", Handle)
        .RequireScopeAuthorization("read:accounts");

    private static IResult Handle(
        HttpContext httpContext,
        UriGenerator uriGenerator,
        AppDbContext appDbContext,
        SettingsService settingsService,
        CancellationToken cancellationToken
    )
    {
        var username = settingsService[SettingsService.UsernameKey];

        return Ok(new Models.Mastodon.CredentialAccount
        {
            Id = "1",
            Username = username,
            AccountName = username,
            DisplayName = settingsService[SettingsService.DisplayNameKey],
            Note = settingsService[SettingsService.SummaryKey],
            Url = uriGenerator.GetProfileUri(username),
            Avatar = uriGenerator.GetUri("/images/avatar.png"),
            AvatarStatic = uriGenerator.GetUri("/images/avatar.png"),
            Header = uriGenerator.GetUri("/images/avatar.png"),
            HeaderStatic = uriGenerator.GetUri("/images/avatar.png"),

            CreatedAt = DateTime.UtcNow,

            Source = new()
            {
                Privacy = Models.Mastodon.Enums.Visibility.Public,
                Sensitive = false,
                Language = "en",
                Note = settingsService[SettingsService.SummaryKey],
                FollowRequestsCount = 0
            }
        });
    }
}
