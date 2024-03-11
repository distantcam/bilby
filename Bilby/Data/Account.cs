using Microsoft.EntityFrameworkCore;

namespace Bilby.Data;

[PrimaryKey(nameof(Id))]
[Index(nameof(ActorUrl), IsUnique = true)]
public record Account
{
    public long Id { get; }

    public required string ActorUrl { get; init; }
    public required string? InboxUrl { get; init; }
    public required string? OutboxUrl { get; init; }
    public required string? FollowersUrl { get; init; }
    public required string? FollowingUrl { get; init; }

    public required string PublicKeyPem { get; init; }
}
