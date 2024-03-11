using Microsoft.EntityFrameworkCore;

namespace Bilby.Data;

[PrimaryKey(nameof(Id))]
public record Application
{
    public long Id { get; }

    public required string ClientName { get; init; }
    public required IList<string> Scopes { get; init; }
    public required string? Website { get; init; }

    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}
