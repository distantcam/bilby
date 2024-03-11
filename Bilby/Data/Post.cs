using Microsoft.EntityFrameworkCore;

namespace Bilby.Data;

[PrimaryKey(nameof(Id))]
public record Post
{
    public long Id { get; }
    public required string Content { get; set; }
    public required DateTime PostedAt { get; set; }
    public List<PostMedia>? Media { get; set; }
    public bool IsDeleted { get; set; }
}

[PrimaryKey(nameof(Id))]
public record PostMedia
{
    public long Id { get; }

    public string Url { get; init; } = default!;
    public string? Alt { get; init; } = default!;

    public long PostId { get; init; }
    public Post Post { get; init; } = default!;

    public PostMedia(string url, string? alt = null)
    {
        Url = url;
        Alt = alt;
    }
}
