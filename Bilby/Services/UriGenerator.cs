using Injectio.Attributes;
using Microsoft.AspNetCore.Http.Extensions;

namespace Bilby.Services;

[RegisterSingleton]
public class UriGenerator(
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration)
{
    public string GetHostName() => GetCurrentContext().Request.Host.Value;

    public string GetCurrentUri() =>
        GetCurrentContext().Request.GetDisplayUrl();

    public string GetUri(string relativePath = "")
    {
        ArgumentNullException.ThrowIfNull(relativePath);
        return GetRootUri() + relativePath;
    }

    public string GetProfileUri(string username) => GetUri("/@" + username);

    public string GetPostUri(long postId) => GetUri($"/post/{postId}");
    public long GetPostId(string uri) => long.Parse(new Uri(uri).Segments[^1]);

    private string GetRootUri() =>
        GetCurrentContext().Request.Scheme + Uri.SchemeDelimiter + GetCurrentContext().Request.Host.Value;

    private HttpContext GetCurrentContext()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null)
            throw new InvalidOperationException("Not in HTTP context");

        var scheme = configuration["scheme"];
        if (scheme != null)
        {
            httpContext.Request.Scheme = scheme;
        }

        var host = configuration["host"];
        if (host != null)
        {
            httpContext.Request.Host = new HostString(host);
        }

        return httpContext;
    }
}
