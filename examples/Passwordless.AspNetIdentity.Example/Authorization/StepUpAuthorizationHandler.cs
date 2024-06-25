using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Passwordless.AspNetIdentity.Example.Authorization;

public interface IStepUpAuthorizationRequirement : IAuthorizationRequirement
{
    public string Name { get; }
}

public class StepUpAuthorizationHandler(StepUpPurpose stepUpPurpose, TimeProvider timeProvider) : AuthorizationHandler<IStepUpAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IStepUpAuthorizationRequirement requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            return Task.CompletedTask;
        }

        if (context.User.HasClaim(MatchesClaim(requirement))
            && IsExpired(GetExpiration(context.User.FindFirst(MatchesClaim(requirement))!)))
        {
            context.Succeed(requirement);
        }
        else
        {
            stepUpPurpose.Purpose = requirement.Name;
            context.Fail();
        }

        return Task.CompletedTask;
    }

    private static Predicate<Claim> MatchesClaim(IStepUpAuthorizationRequirement requirement) => claim => claim.Type == requirement.Name;

    private bool IsExpired(DateTime expiration)
    {
        return expiration > timeProvider.GetUtcNow().DateTime;
    }

    private static DateTime GetExpiration(Claim claim)
    {
        var expiration = DateTime.Parse(claim.Value, null, DateTimeStyles.RoundtripKind);

        return expiration;
    }
}