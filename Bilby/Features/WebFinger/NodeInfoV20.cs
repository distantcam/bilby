using System.Reflection;
using EndpointGenerator;

namespace Bilby.Features.WebFinger;

public static class NodeInfoV20
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder)
    {
        builder
            .MapGet("/nodeinfo/2.0.json", Handler)
            .WithName("NodeinfoV2_A")
            .Produces<ServerInfo>(200);

        builder
            .MapGet("/nodeinfo/2.0", Handler)
            .WithName("NodeinfoV2_B")
            .Produces<ServerInfo>(200);
    }

    public record struct ServerInfo(string Version, Software Software, string[] Protocols, Services Services, bool OpenRegistrations, Usage Usage);
    public record struct Software(string Name, string Version);
    public record struct Services(string[] Inbound, string[] Outbound);
    public record struct Usage(UserCounts Users, int LocalPosts);
    public record struct UserCounts(int Total, int ActiveHalfyear, int ActiveMonth);

    private static IResult Handler()
    {
        var assemblyName = Assembly.GetExecutingAssembly()?.GetName();
        var software = assemblyName == null
            ? new Software("bilby", "0.0.0")
            : new Software("bilby", assemblyName.Version?.ToString(3) ?? "0.0.0");

        var response = new ServerInfo(
            Version: "2.0",
            software,
            ["activitypub"],
            new Services([], []),
            false,
            new Usage(new UserCounts(1, 1, 1), 0)
        );

        return Ok(response);
    }
}
