using Injectio.Attributes;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Net.Http.Headers;

namespace Bilby.Middleware;

public interface IAcceptHeaderMetadata
{
    IReadOnlyList<string> ContentTypes { get; }
}

[RegisterSingleton<MatcherPolicy>(Duplicate = DuplicateStrategy.Append)]
internal sealed class AcceptHeaderMatcherPolicy : MatcherPolicy, IEndpointComparerPolicy, INodeBuilderPolicy, IEndpointSelectorPolicy
{
    private const string AnyContentType = "*/*";

    public override int Order { get; } = -100;

    public IComparer<Endpoint> Comparer { get; } = new AcceptsMetadataEndpointComparer();

    bool INodeBuilderPolicy.AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        if (ContainsDynamicEndpoints(endpoints))
        {
            return false;
        }

        return AppliesToEndpointsCore(endpoints);
    }

    bool IEndpointSelectorPolicy.AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        // When the node contains dynamic endpoints we can't make any assumptions.
        return ContainsDynamicEndpoints(endpoints);
    }

    private static bool AppliesToEndpointsCore(IReadOnlyList<Endpoint> endpoints)
    {
        return endpoints.Any(e => e.Metadata.GetMetadata<IAcceptHeaderMetadata>()?.ContentTypes.Count > 0);
    }

    public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(candidates);

        for (var i = 0; i < candidates.Count; i++)
        {
            if (!candidates.IsValidCandidate(i))
            {
                continue;
            }

            var acceptsMeta = candidates[i].Endpoint.Metadata.GetMetadata<IAcceptHeaderMetadata>()?.ContentTypes;
            if (acceptsMeta == null || acceptsMeta.Count == 0)
            {
                continue;
            }

            var matched = false;
            var accepts = GetAcceptMimeTypes(httpContext);
            for (var j = 0; !matched && j < accepts.Count; j++)
            {
                var acceptMediaType = new MediaTypeHeaderValue(accepts[j]);
                for (var k = 0; !matched && k < acceptsMeta.Count; k++)
                {
                    var destination = new MediaTypeHeaderValue(acceptsMeta[k]);
                    if (acceptMediaType.IsSubsetOf(destination))
                    {
                        matched = true;
                        break;
                    }
                }
            }

            if (!matched)
            {
                candidates.SetValidity(i, false);
            }
        }

        return Task.CompletedTask;
    }

    public IReadOnlyList<PolicyNodeEdge> GetEdges(IReadOnlyList<Endpoint> endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var edges = new Dictionary<string, List<Endpoint>>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < endpoints.Count; i++)
        {
            var endpoint = endpoints[i];
            var contentTypes = endpoint.Metadata.GetMetadata<IAcceptHeaderMetadata>()?.ContentTypes;
            if (contentTypes == null || contentTypes.Count == 0)
            {
                contentTypes = [AnyContentType];
            }

            for (var j = 0; j < contentTypes.Count; j++)
            {
                var contentType = contentTypes[j];

                if (!edges.ContainsKey(contentType))
                {
                    edges.Add(contentType, []);
                }
            }
        }

        for (var i = 0; i < endpoints.Count; i++)
        {
            var endpoint = endpoints[i];
            var contentTypes = endpoint.Metadata.GetMetadata<IAcceptHeaderMetadata>()?.ContentTypes ?? Array.Empty<string>();
            if (contentTypes.Count == 0)
            {
                foreach (var kvp in edges)
                {
                    kvp.Value.Add(endpoint);
                }
            }
            else
            {
                foreach (var kvp in edges)
                {
                    var edgeKey = new MediaTypeHeaderValue(kvp.Key);

                    for (var j = 0; j < contentTypes.Count; j++)
                    {
                        var contentType = contentTypes[j];
                        var mediaType = new MediaTypeHeaderValue(contentType);
                        if (edgeKey.IsSubsetOf(mediaType))
                        {
                            kvp.Value.Add(endpoint);
                            break;
                        }
                    }
                }
            }
        }

        var result = new PolicyNodeEdge[edges.Count];
        var index = 0;
        foreach (var kvp in edges)
        {
            result[index] = new PolicyNodeEdge(kvp.Key, kvp.Value);
            index++;
        }
        return result;
    }

    public PolicyJumpTable BuildJumpTable(int exitDestination, IReadOnlyList<PolicyJumpTableEdge> edges)
    {
        ArgumentNullException.ThrowIfNull(edges);

        var ordered = new (MediaTypeHeaderValue mediaType, int destination)[edges.Count];
        for (var i = 0; i < edges.Count; i++)
        {
            var e = edges[i];
            ordered[i] = (mediaType: new MediaTypeHeaderValue((string)e.State), destination: e.Destination);
        }
        Array.Sort(ordered, static (left, right) => GetScore(left.mediaType).CompareTo(GetScore(right.mediaType)));

        return new AcceptsPolicyJumpTable(exitDestination, ordered);
    }

    private static int GetScore(MediaTypeHeaderValue mediaType)
    {
        if (mediaType.MatchesAllTypes)
        {
            return 4;
        }
        else if (mediaType.MatchesAllSubTypes)
        {
            return 3;
        }
        else if (mediaType.MatchesAllSubTypesWithoutSuffix)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    private static List<string> GetAcceptMimeTypes(HttpContext httpContext) =>
        httpContext.Request.Headers.Accept.SelectMany(a => a?.Split(',') ?? []).ToList();

    private sealed class AcceptsMetadataEndpointComparer : EndpointMetadataComparer<IAcceptHeaderMetadata>
    {
        protected override int CompareMetadata(IAcceptHeaderMetadata? x, IAcceptHeaderMetadata? y)
        {
            // Ignore the metadata if it has an empty list of content types.
            return base.CompareMetadata(
                x?.ContentTypes.Count > 0 ? x : null,
                y?.ContentTypes.Count > 0 ? y : null);
        }
    }

    private sealed class AcceptsPolicyJumpTable(int exitDestination, (MediaTypeHeaderValue mediaType, int destination)[] destinations) : PolicyJumpTable
    {
        public override int GetDestination(HttpContext httpContext)
        {
            var accepts = GetAcceptMimeTypes(httpContext);
            if (accepts.Count == 0)
            {
                accepts = [AnyContentType];
            }

            for (var i = 0; i < accepts.Count; i++)
            {
                var acceptMediaType = MediaTypeHeaderValue.Parse(accepts[i]);
                for (var j = 0; j < destinations.Length; j++)
                {
                    var destination = destinations[j].mediaType;
                    if (acceptMediaType.IsSubsetOf(destination))
                    {
                        return destinations[j].destination;
                    }
                }
            }

            return exitDestination;
        }
    }
}
