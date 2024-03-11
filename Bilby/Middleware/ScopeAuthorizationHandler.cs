using System.Diagnostics.CodeAnalysis;
using Injectio.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Bilby.Middleware;

public interface IAuthRequiredScopeMetadata
{
    IReadOnlyList<string> AcceptedScope { get; }
}

public record ScopeAuthorizationRequirement(IEnumerable<string>? AllowedValues = null) : IAuthorizationRequirement;

[RegisterSingleton<IAuthorizationHandler>(Duplicate = DuplicateStrategy.Append)]
internal sealed class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationRequirement>
{
    private static readonly ScopeComparer s_scopeComparer = new();

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeAuthorizationRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var endpoint = context.Resource switch
        {
            HttpContext httpContext => httpContext.GetEndpoint(),
            Endpoint ep => ep,
            _ => null,
        };

        var data = endpoint?.Metadata.GetMetadata<IAuthRequiredScopeMetadata>();

        var scopes = requirement.AllowedValues ?? data?.AcceptedScope;

        if (scopes is null)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var scopeClaims = context.User.FindAll("scopes").ToList();

        if (scopeClaims.Count == 0)
        {
            return Task.CompletedTask;
        }

        var hasScope = scopeClaims.SelectMany(s => s.Value.Split(' '))
            .Intersect(scopes, s_scopeComparer).Any();

        if (hasScope)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    private class ScopeComparer : IEqualityComparer<string>
    {
        private static readonly StringComparer s_baseComparer = StringComparer.OrdinalIgnoreCase;

        public int GetHashCode([DisallowNull] string obj)
        {
            return s_baseComparer.GetHashCode(obj.Split(':')[0]);
        }

        public bool Equals(string? x, string? y)
        {
            return s_baseComparer.Equals(x, y) ||
                s_baseComparer.Equals(x?.Split(':')[0], y) ||
                s_baseComparer.Equals(x, y?.Split(':')[0]);
        }
    }
}
