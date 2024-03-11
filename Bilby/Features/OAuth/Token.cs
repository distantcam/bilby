using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bilby.Models;
using EndpointGenerator;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Bilby.Features.OAuth;

public static class Token
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapPost("/oauth/token", Handle)
        .DisableAntiforgery();

    public record TokenRequest(
        [property: JsonPropertyName("grant_type")] string GrantType,
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("client_id")] string ClientId,
        [property: JsonPropertyName("client_secret")] string? ClientSecret,
        [property: JsonPropertyName("redirect_uri")] string RedirectUri,
        [property: JsonPropertyName("scope")] string? Scope,
        [property: JsonPropertyName("me")] string? Me
    )
    {
        public static ValueTask<TokenRequest?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
        {
            var req = httpContext.Request;

            if (req.HasJsonContentType())
            {
                return httpContext.Request.ReadFromJsonAsync<TokenRequest>();
            }

            if (req.HasFormContentType)
            {
                if (!httpContext.Request.Form.TryGetValue("grant_type", out var grantType))
                    return ValueTask.FromResult<TokenRequest?>(null);
                if (!httpContext.Request.Form.TryGetValue("code", out var code))
                    return ValueTask.FromResult<TokenRequest?>(null);
                if (!httpContext.Request.Form.TryGetValue("client_id", out var clientId))
                    return ValueTask.FromResult<TokenRequest?>(null);
                if (!httpContext.Request.Form.TryGetValue("redirect_uri", out var redirectUri))
                    return ValueTask.FromResult<TokenRequest?>(null);
                httpContext.Request.Form.TryGetValue("client_secret", out var clientSecret);
                httpContext.Request.Form.TryGetValue("scope", out var scope);
                httpContext.Request.Form.TryGetValue("me", out var me);
                return ValueTask.FromResult<TokenRequest?>(new(
                    GrantType: grantType!,
                    Code: code!,
                    ClientId: clientId!,
                    ClientSecret: clientSecret,
                    RedirectUri: redirectUri!,
                    Scope: scope,
                    Me: me
                ));
            }

            return ValueTask.FromResult<TokenRequest?>(null);
        }
    }

    private static IResult Handle(
        TokenRequest request,
        HttpContext httpContext,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<JwtConfig> jwtOptions
    )
    {
        if (!StringComparer.OrdinalIgnoreCase.Equals(request.GrantType, "authorization_code"))
            return BadRequest();

        var protector = dataProtectionProvider.CreateProtector("oauth");
        var codeString = protector.Unprotect(request.Code ?? throw new Exception("Missing code"));
        var authCode = JsonSerializer.Deserialize(codeString, AppJsonSerializerContext.Default.AuthCode);

        if (!StringComparer.OrdinalIgnoreCase.Equals(authCode.RedirectUri, request.RedirectUri))
            return BadRequest();

        if (!StringComparer.OrdinalIgnoreCase.Equals(authCode.Me, request.Me))
            return BadRequest();

        var scopes = authCode.Scopes ?? [];

        List<Claim> claims = [
            new(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString("N")),
            new("clientId", request.ClientId)
        ];

        foreach (var scope in scopes)
        {
            claims.Add(new("scopes", scope));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(30),
            Issuer = jwtOptions.Value.Issuer,
            Audience = jwtOptions.Value.Audience,
            SigningCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key)), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        if (authCode.RedirectUri.Equals("urn:ietf:wg:oauth:2.0:oob", OrdinalIgnoreCase))
            return Ok(jwtToken);

        var response = new TokenResponse(
            AccessToken: jwtToken,
            TokenType: "bearer",
            ExpiresIn: 3600,
            RefreshToken: null,
            Scope: string.Join(' ', scopes),
            CreatedAt: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Me: request.Me
        );

        var jsonMediaType = MediaTypeHeaderValue.Parse(MimeTypes.Json);
        var formMediaType = MediaTypeHeaderValue.Parse(MimeTypes.Form);
        var acceptTypes = ((string?)httpContext.Request.Headers.Accept ?? "").Split(',')
            .Select(s => MediaTypeHeaderValue.Parse(s))
            .ToArray();

        if (acceptTypes.Any(jsonMediaType.IsSubsetOf))
        {
            return Ok(response);
        }

        if (acceptTypes.Any(formMediaType.IsSubsetOf))
        {
            var body = $"access_token={response.AccessToken}&scope={response.Scope}";
            if (!string.IsNullOrEmpty(request.Me))
                body += $"&me={request.Me}";
            return Ok(body);
        }

        return BadRequest();
    }

    public record struct TokenResponse(
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("expires_in")] long? ExpiresIn,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("scope")] string? Scope,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("created_at")] long CreatedAt,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [property: JsonPropertyName("me")] string? Me // micropub
    );
}
