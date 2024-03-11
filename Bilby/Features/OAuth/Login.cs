using System.Security.Claims;
using System.Web;
using Bilby.Data;
using Bilby.Services;
using EndpointGenerator;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bilby.Features.OAuth;

public static class Login
{
    [EndpointGroupBuilder]
    public static void MapEndpoint(RouteGroupBuilder builder)
    {
        builder.MapGet("/oauth/login", GetLogin).WithName(nameof(GetLogin));
        builder.MapPost("/oauth/login", PostLogin).WithName(nameof(PostLogin));
    }

    private static IResult GetLogin(
        [FromQuery] string returnUrl,
        SettingsService settingsService,
        UriGenerator uriGenerator,
        HttpContext context,
        IAntiforgery antiforgery
    )
    {
        var token = antiforgery.GetAndStoreTokens(context);
        var page = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Login</title>

  <style>
      *,
      *::before,
      *::after {{
        box-sizing: border-box;
      }}

      * {{
        margin: 0;
        padding: 0;
        font: inherit;
        font-family: sans-serif;
      }}

      html {{
        color-scheme: dark light;
      }}

      body {{
        min-height: 100vh;
        min-width: 100vw;
        display: grid;
        place-content: center;
      }}

      h1 {{
        font-size: 2rem;
        font-weight: bold;
      }}

      form {{
        display: grid;
        gap: 1rem;
        width: min(20rem, 100%);
        text-align: center;
      }}

      input,
      button {{
        padding: 0.5rem 0.75rem;
      }}

      button {{
        cursor: pointer;
        border-radius: 2rem;
      }}
    </style>
</head>
<body>

  <form action=""/oauth/login?returnUrl={HttpUtility.UrlEncode(returnUrl)}"" method=""post"">
    <h1>
      <img src=""/images/avatar.png"" width=""80"" height=""80"" />
      <br />
      {settingsService[SettingsService.UsernameKey]}
    </h1>
    <input name=""{token.FormFieldName}"" type=""hidden"" value=""{token.RequestToken}"" />
    <input id=""password"" name=""password"" required type=""password"" placeholder=""password"" />
    <button>Login</button>
  </form>

</body>
</html>
";
        return Content(page, "text/html");
    }

    private static async Task<IResult> PostLogin(
        [FromQuery] string returnUrl,
        [FromForm] string password,
        HttpContext context,
        AppDbContext appDbContext,
        SettingsService settingsService,
        IPasswordHasher passwordHasher
    )
    {
        var uri = new Uri("http://0.0.0.0" + returnUrl);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var clientId = query.Get("client_id");

        if (string.IsNullOrEmpty(clientId))
            return BadRequest();

        var storedPasswordHash = settingsService[SettingsService.PasswordKey];
        var verified = passwordHasher.VerifyHashedPassword(storedPasswordHash, password);
        if (verified == PasswordVerificationResult.Failed)
            return Unauthorized();

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim("clientId", clientId)],
                    CookieAuthenticationDefaults.AuthenticationScheme
        )));

        return Redirect(returnUrl);
    }
}
