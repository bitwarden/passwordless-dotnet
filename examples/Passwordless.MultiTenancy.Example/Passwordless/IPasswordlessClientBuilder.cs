namespace Passwordless.MultiTenancy.Example.Passwordless;

public interface IPasswordlessClientBuilder
{
    PasswordlessClientBuilder WithApiUrl(string apiUrl);

    PasswordlessClientBuilder WithTenant(string tenant);

    PasswordlessClient Build();
}