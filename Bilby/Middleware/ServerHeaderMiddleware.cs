namespace Bilby.Middleware;

public static class ServerHeaderMiddleware
{
    public static IApplicationBuilder UseServerHeader(this IApplicationBuilder app) =>
        app.UseMiddleware<Middleware>();

    private class Middleware(RequestDelegate next)
    {
        public Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Append("Server", "Bilby");
            return next(context);
        }
    }
}
