using System.Text;
using Bilby.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.IdentityModel.Tokens;

namespace Bilby.Startup;

public static class AuthExtensions
{
    public static IServiceCollection AddBilbyAuth(this IServiceCollection services, string issuer, string audience, string key)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultScheme = "JWT_OR_COOKIE";
            o.DefaultChallengeScheme = "JWT_OR_COOKIE";
            o.DefaultAuthenticateScheme = "JWT_OR_COOKIE";
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            o.TokenValidationParameters = new()
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
        {
            o.LoginPath = "/oauth/login";
        })
        .AddScheme<SignatureAuthenticationOptions, SignatureAuthenticationHandler>(
            SignatureAuthentication.Schema, o => { })
        .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", o =>
        {
            o.ForwardDefaultSelector = context =>
            {
                string? authorization = context.Request.Headers.Authorization;
                if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                    return JwtBearerDefaults.AuthenticationScheme;

                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.AddRequirements(new ScopeAuthorizationRequirement());
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy(CookieAuthenticationDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy(SignatureAuthentication.Schema, policy =>
            {
                policy.AddAuthenticationSchemes(SignatureAuthentication.Schema);
                policy.RequireClaim("signed", "true");
            });

        var policyEvaluator = services.Where(s => s.ServiceType == typeof(IPolicyEvaluator)).ToArray();

        return services;
    }
}
