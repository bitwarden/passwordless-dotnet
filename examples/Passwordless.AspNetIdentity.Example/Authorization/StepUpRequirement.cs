namespace Passwordless.AspNetIdentity.Example.Authorization;

public class StepUpRequirement(string policyName) : IStepUpAuthorizationRequirement
{
    public string Name => policyName;
}