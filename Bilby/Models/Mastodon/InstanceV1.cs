using System.Text.Json.Serialization;

namespace Bilby.Models.Mastodon;

public record InstanceV1
{
    [JsonPropertyName("uri")] public required string Uri { get; init; }
    [JsonPropertyName("title")] public required string Title { get; init; }
    [JsonPropertyName("short_description")] public required string ShortDescription { get; init; }
    [JsonPropertyName("description")] public required string Description { get; init; }
    [JsonPropertyName("email")] public required string Email { get; init; }
    [JsonPropertyName("version")] public required string Version { get; init; }
    [JsonPropertyName("urls")] public required UrlsHash Urls { get; init; }
    [JsonPropertyName("stats")] public required StatsHash Stats { get; init; }
    [JsonPropertyName("thumbnail")] public string? Thumbnail { get; init; }
    [JsonPropertyName("languages")] public required IEnumerable<string> Languages { get; init; }
    [JsonPropertyName("registrations")] public bool Registrations { get; init; }
    [JsonPropertyName("approval_required")] public bool ApprovalRequired { get; init; }
    [JsonPropertyName("invites_enabled")] public bool InvitesEnabled { get; init; }
    [JsonPropertyName("configuration")] public required ConfigurationHash Configuration { get; init; }
    [JsonPropertyName("contact_account")] public required Account ContactAccount { get; init; }
    [JsonPropertyName("rules")] public required IEnumerable<Rule> Rules { get; init; }

    public record UrlsHash
    {
        [JsonPropertyName("streaming_api")] public required string StreamingApi { get; init; }
    }

    public record StatsHash
    {
        [JsonPropertyName("user_count")] public required int UserCount { get; init; }
        [JsonPropertyName("status_count")] public required int StatusCount { get; init; }
        [JsonPropertyName("domain_count")] public required int DomainCount { get; init; }
    }

    public record ConfigurationHash
    {
        [JsonPropertyName("accounts")] public required AccountsHash Accounts { get; init; }
        [JsonPropertyName("statuses")] public required StatusesHash Statuses { get; init; }
        [JsonPropertyName("media_attachments")] public required MediaAttachmentsHash MediaAttachments { get; init; }
        [JsonPropertyName("polls")] public required PollsHash Polls { get; init; }

        public record AccountsHash
        {
            [JsonPropertyName("max_featured_tags")] public required int MaxFeaturedTags { get; init; }
        }

        public record StatusesHash
        {
            [JsonPropertyName("max_characters")] public required int MaxCharacters { get; init; }
            [JsonPropertyName("max_media_attachments")] public required int MaxMediaAttachments { get; init; }
            [JsonPropertyName("characters_reserved_per_url")] public required int CharactersReservedPerUrl { get; init; }
        }

        public record MediaAttachmentsHash
        {
            [JsonPropertyName("supported_mime_types")] public required IEnumerable<string> SupportedMimeTypes { get; init; }
            [JsonPropertyName("image_size_limit")] public required int ImageSizeLimit { get; init; }
            [JsonPropertyName("image_matrix_limit")] public required int ImageMatrixLimit { get; init; }
            [JsonPropertyName("video_size_limit")] public required int VideoSizeLimit { get; init; }
            [JsonPropertyName("video_frame_rate_limit")] public required int VideoFrameRateLimit { get; init; }
            [JsonPropertyName("video_matrix_limit")] public required int VideoMatrixLimit { get; init; }
        }

        public record PollsHash
        {
            [JsonPropertyName("max_options")] public required int MaxOptions { get; init; }
            [JsonPropertyName("max_characters_per_option")] public required int MaxCharactersPerOption { get; init; }
            [JsonPropertyName("min_expiration")] public required int MinExpiration { get; init; }
            [JsonPropertyName("max_expiration")] public required int MaxExpiration { get; init; }

        }
    }
}
