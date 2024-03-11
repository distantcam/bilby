using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoCtor;
using Bilby.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;

namespace Bilby.Middleware;

public static class SignatureAuthentication
{
    // Mastodon Signature Authentication
    // https://docs.joinmastodon.org/spec/security/
    public static readonly string Schema = "SignatureHeader";
}

public class SignatureAuthenticationOptions : AuthenticationSchemeOptions
{
}

[AutoConstruct]
public partial class SignatureAuthenticationHandler : AuthenticationHandler<SignatureAuthenticationOptions>
{
    private static readonly string[] s_separator = ["SHA-256="];

    private readonly ILogger<SignatureAuthenticationHandler> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly AccountLookupManager _accountLookupManager;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.EnableBuffering();

        var requestHeaders = Request.Headers
            .ToDictionary(h => h.Key.ToLowerInvariant(), h => (string)h.Value!);

        try
        {
            Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            if (requestHeaders.TryGetValue("digest", out var digest) &&
                !CheckDigest(digest, body))
                return AuthenticateResult.Fail("Bad digest");
        }
        finally
        {
            Request.Body.Position = 0;
        }

        if (requestHeaders.TryGetValue("signature", out var signature) &&
            !await CheckSignature(signature, Request.Method, Request.Path, Request.QueryString.ToString(), requestHeaders))
            return AuthenticateResult.Fail("Bad signature");

        var claims = new[] {
            new Claim("signed", "true")
        };
        var claimsIdentity = new ClaimsIdentity(claims, nameof(SignatureAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private static bool CheckDigest(string digest, string body)
    {
        var digestHash = digest.Split(s_separator, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(body));
        var calculatedDigestHash = Convert.ToBase64String(bytes);
        return digestHash == calculatedDigestHash;
    }

    private async Task<bool> CheckSignature(string signature, string method, string path, string queryString, Dictionary<string, string> requestHeaders)
    {
        var sigRegex = SignatureRegex();
        var signatureHeaders = signature.Split(',')
            .Select(x => sigRegex.Match(x))
            .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);

        var keyId = signatureHeaders["keyId"];
        var headers = signatureHeaders["headers"];
        var algorithm = signatureHeaders["algorithm"];
        var sig = Convert.FromBase64String(signatureHeaders["signature"]);
        var actorId = new Uri(keyId).GetLeftPart(UriPartial.Path);

        var toSign = Encoding.UTF8.GetBytes(string.Join('\n', headers.Split(' ')
            .Select(headerKey =>
            {
                if (headerKey == "(request-target)")
                    return $"(request-target): {method.ToLower()} {path}{queryString}";
                return $"{headerKey}: {requestHeaders[headerKey]}";
            })));

        var publicKeyPem = await _distributedCache.GetStringAsync(keyId);
        var result = VerifySignature(publicKeyPem, algorithm, toSign, sig);
        if (!result)
        {
            // Maybe the cached version is bad. Try getting the key again.
            await _distributedCache.RemoveAsync(keyId);
            publicKeyPem = await _accountLookupManager.GetAccountPublicKey(actorId);
            if (!string.IsNullOrEmpty(publicKeyPem))
                await _distributedCache.SetStringAsync(keyId, publicKeyPem);
            result = VerifySignature(publicKeyPem, algorithm, toSign, sig);
        }

        return result;
    }

    private bool VerifySignature(string? publicKeyPem, string algorithm, byte[] toSign, byte[] sig)
    {
        if (string.IsNullOrEmpty(publicKeyPem))
            return false;

        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        if (algorithm.Equals("rsa-sha256", StringComparison.InvariantCultureIgnoreCase))
        {
            return rsa.VerifyData(toSign, sig, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        throw new NotImplementedException("Unknown signature algorithm: " + algorithm);
    }

    [GeneratedRegex("^([a-zA-Z0-9]+)=\"(.+)\"$")]
    private static partial Regex SignatureRegex();
}
