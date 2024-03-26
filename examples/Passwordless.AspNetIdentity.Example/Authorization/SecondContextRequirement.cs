namespace Passwordless.AspNetIdentity.Example.Authorization;

public class SecondContextRequirement : IStepUpAuthorizationRequirement
{
    public const string PolicyName = "SecondContext";
    public string Name => PolicyName;
}