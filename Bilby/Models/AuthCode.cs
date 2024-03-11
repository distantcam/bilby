namespace Bilby.Models;

public record struct AuthCode(
    string ClientId,
    string? RedirectUri,
    IEnumerable<string>? Scopes,
    string? Me
);
