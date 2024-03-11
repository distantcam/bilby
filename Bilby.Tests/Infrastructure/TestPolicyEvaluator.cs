using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Bilby.Tests.Infrastructure;

public class TestPolicyEvaluator(IAuthorizationService authorization) : IPolicyEvaluator
{
    private readonly PolicyEvaluator _basePolicyEvaluator = new(authorization);

    public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        return await _basePolicyEvaluator.AuthenticateAsync(policy, context);
    }

    public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        if (context.Request.Headers.TryGetValue("x-testing", out var testing) &&
            testing.Equals("true"))
        {
            return PolicyAuthorizationResult.Success();
        }

        return await _basePolicyEvaluator.AuthorizeAsync(policy, authenticationResult, context, resource);
    }
}
