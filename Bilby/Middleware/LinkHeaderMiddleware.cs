namespace Bilby.Middleware;

public static class LinkHeaderMiddleware
{
    public static IApplicationBuilder UseLinkAuthHeaders(this IApplicationBuilder app) =>
        app.UseMiddleware<Middleware>();

    private class Middleware(RequestDelegate next)
    {
        public Task InvokeAsync(HttpContext context)
        {
            var rootUri = context.Request.Scheme + Uri.SchemeDelimiter + context.Request.Host.Value;

            context.Response.Headers.Append("Link", $"<{rootUri}/oauth/authorize>; rel=\"authorization_endpoint\"");
            context.Response.Headers.Append("Link", $"<{rootUri}/oauth/token>; rel=\"token_endpoint\"");
            context.Response.Headers.Append("Link", $"<{rootUri}/micropub>; rel=\"micropub\"");

            return next(context);
        }
    }
}
