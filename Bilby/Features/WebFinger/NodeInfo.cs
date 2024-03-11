using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.WebFinger;

public static class NodeInfo
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/.well-known/nodeinfo", Handler)
        .Produces<NodeLinks>(200);

    public record struct NodeLinks(IEnumerable<NodeLink> Links);
    public record struct NodeLink(string Rel, string Href);

    private static IResult Handler(UriGenerator uriGenerator)
    {
        var nodeV2Url = uriGenerator.GetUri("/nodeinfo/2.0.json");
        var response = new NodeLinks([
            new("http://nodeinfo.diaspora.software/ns/schema/2.0", nodeV2Url)
        ]);
        return Ok(response);
    }
}
