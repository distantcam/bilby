using Bilby.Middleware;
using Bilby.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Metadata;

namespace Microsoft.AspNetCore.Builder;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder RequireAcceptActivityStream(this RouteHandlerBuilder builder)
    {
        return builder.RequireAccept(MimeTypes.ActivityJson, MimeTypes.LdJsonMimeType);
    }

    public static TBuilder RequireAccept<TBuilder>(
        this TBuilder builder,
        string contentType,
        params string[] additionalContentTypes)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new AcceptHeaderMetadata([contentType, .. additionalContentTypes]));
    }

    public static TBuilder RequireContentType<TBuilder>(
        this TBuilder builder,
        string contentType,
        params string[] additionalContentTypes)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.WithMetadata(new AcceptsMetadata([contentType, .. additionalContentTypes]));
    }

    public static TBuilder RequireScopeAuthorization<TBuilder>(
        this TBuilder endpointConventionBuilder,
        params string[] scope)
        where TBuilder : IEndpointConventionBuilder
    {
        return endpointConventionBuilder
            .WithMetadata(new RequiredScopeMetadata(scope))
            .RequireAuthorization(JwtBearerDefaults.AuthenticationScheme);
    }

    private record AcceptHeaderMetadata(IReadOnlyList<string> ContentTypes) : IAcceptHeaderMetadata;
    private record RequiredScopeMetadata(IReadOnlyList<string> AcceptedScope) : IAuthRequiredScopeMetadata;
}
