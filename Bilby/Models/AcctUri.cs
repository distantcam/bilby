using System.Diagnostics.CodeAnalysis;

namespace Bilby.Models;

public record AcctUri(string User, string Host)
{
    public static bool TryParse(string value, [MaybeNullWhen(false)] out AcctUri result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!value.StartsWith("acct:"))
            return false;

        var parts = value.Substring(5).Split('@');

        if (parts.Length != 2)
            return false;

        var user = parts[0];
        var host = parts[1];

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(host))
            return false;

        result = new AcctUri(user, host);
        return true;
    }

    public override string ToString() => "acct:" + User + "@" + Host;
}
