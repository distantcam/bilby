using System.Diagnostics.CodeAnalysis;

namespace Bilby.Features;

public record ErrorResponse
{
    public required string Error { get; init; }
    public string? ErrorDescription { get; init; }

    [SetsRequiredMembers]
    public ErrorResponse(string error, string? errorDescription = null)
    {
        Error = error;
        ErrorDescription = errorDescription;
    }
}
