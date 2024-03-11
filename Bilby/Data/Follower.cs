using Microsoft.EntityFrameworkCore;

namespace Bilby.Data;

[PrimaryKey(nameof(Id))]
public record Follower
{
    public long Id { get; }
    public required string FollowId { get; init; }
    public long AccountId { get; init; }
    public Account Account { get; init; } = default!;
}
