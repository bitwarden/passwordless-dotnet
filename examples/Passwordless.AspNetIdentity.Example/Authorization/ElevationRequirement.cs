namespace Passwordless.AspNetIdentity.Example.Authorization;

public class ElevationRequirement : IStepUpAuthorizationRequirement
{
    public const string PolicyName = "Elevated";
    public string Name => PolicyName;
}