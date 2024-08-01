using Microsoft.Extensions.Options;

namespace Passwordless.MultiTenancy.Example.Passwordless;

public class PasswordlessClientBuilder : IPasswordlessClientBuilder
{
    private readonly PasswordlessOptions _options = new()
    {
        ApiSecret = null!
    };
    private readonly PasswordlessMultiTenancyConfiguration _multiTenancyOptions;

    public PasswordlessClientBuilder(IOptions<PasswordlessMultiTenancyConfiguration> multiTenancyOptions)
    {
        _multiTenancyOptions = multiTenancyOptions.Value ?? throw new ArgumentNullException(nameof(multiTenancyOptions));
    }

    public PasswordlessClientBuilder WithApiUrl(string apiUrl)
    {
        _options.ApiUrl = apiUrl;
        return this;
    }

    public PasswordlessClientBuilder WithTenant(string tenant)
    {
        var tenantConfiguration = _multiTenancyOptions.Tenants[tenant];
        _options.ApiSecret = tenantConfiguration.ApiSecret;
        return this;
    }

    public PasswordlessClient Build()
    {
        return new PasswordlessClient(_options);
    }
}