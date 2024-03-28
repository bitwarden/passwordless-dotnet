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

public class StepUpAuthorizationHandler(StepUpContext stepUpContext) : AuthorizationHandler<IStepUpAuthorizationRequirement>
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
            stepUpContext.Purpose = requirement.Name;
            context.Fail();
        }

        return Task.CompletedTask;
    }

    private static Predicate<Claim> MatchesClaim(IStepUpAuthorizationRequirement requirement) => claim => claim.Type == requirement.Name;

    private static bool IsExpired(DateTime expiration)
    {
        return expiration > DateTime.UtcNow;
    }

    private static DateTime GetExpiration(Claim claim)
    {
        var expiration = DateTime.Parse(claim.Value, null, DateTimeStyles.RoundtripKind);

        return expiration;
    }
}