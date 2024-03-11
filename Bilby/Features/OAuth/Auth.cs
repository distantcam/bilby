using System.Text.Json;
using Bilby.Models;
using EndpointGenerator;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Bilby.Features.OAuth;

public static class Auth
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder) => builder
        .MapGet("/oauth/authorize", Handle)
        .RequireAuthorization(CookieAuthenticationDefaults.AuthenticationScheme);

    private static IResult Handle(
        [FromQuery(Name = "response_type")] string responseType,
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "redirect_uri")] string redirectUri,
        [FromQuery(Name = "scope")] string? scope,
        [FromQuery(Name = "state")] string? state,
        [FromQuery(Name = "me")] string? me,
        IDataProtectionProvider dataProtectionProvider
    )
    {
        if (!StringComparer.OrdinalIgnoreCase.Equals(responseType, "code"))
            return BadRequest();

        var protector = dataProtectionProvider.CreateProtector("oauth");
        var authCode = new AuthCode(
            clientId,
            redirectUri,
            scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            me
        );
        var codeString = protector.Protect(JsonSerializer.Serialize(authCode, AppJsonSerializerContext.Default.AuthCode));

        if (redirectUri.Equals("urn:ietf:wg:oauth:2.0:oob", OrdinalIgnoreCase))
            return Ok(codeString);

        var responseUri = $"{redirectUri}?code={codeString}";
        if (!string.IsNullOrEmpty(state))
        {
            responseUri += $"&state={state}";
        }
        return Redirect(responseUri);
    }
}
