using Bilby.Services;
using EndpointGenerator;

namespace Bilby.Features.WebFinger;

public static class HostMeta
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/.well-known/host-meta", Handle)
        .Produces(200, contentType: "application/xrd+xml; charset=utf-8");

    private static readonly string _template = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<XRD
  xmlns=""http://docs.oasis-open.org/ns/xri/xrd-1.0"">
  <Link rel=""lrdd"" template=""[[SERVER_URI]]/.well-known/webfinger?resource={uri}""/>
</XRD>
";

    private static IResult Handle(UriGenerator uriGenerator)
    {
        var result = _template.Replace("[[SERVER_URI]]", uriGenerator.GetUri());
        return Text(result, contentType: "application/xrd+xml; charset=utf-8");
    }
}
