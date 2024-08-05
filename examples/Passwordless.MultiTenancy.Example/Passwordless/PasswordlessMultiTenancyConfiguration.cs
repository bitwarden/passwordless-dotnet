namespace Passwordless.MultiTenancy.Example.Passwordless;

public class PasswordlessMultiTenancyConfiguration
{
    public Dictionary<string, TenantConfiguration> Tenants { get; set; } = new();
}