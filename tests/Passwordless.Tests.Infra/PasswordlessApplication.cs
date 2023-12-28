using Microsoft.Extensions.DependencyInjection;

namespace Passwordless.Tests.Infra;

public class PasswordlessApplication
{
    public string Name { get; }

    public string ApiUrl { get; }

    public string ApiSecret { get; }

    public string ApiKey { get; }

    public PasswordlessApplication(string name, string apiUrl, string apiSecret, string apiKey)
    {
        Name = name;
        ApiUrl = apiUrl;
        ApiSecret = apiSecret;
        ApiKey = apiKey;
    }

    public IPasswordlessClient CreateClient() =>
        // Initialize using a service container to cover more code paths in tests
        new ServiceCollection().AddPasswordlessSdk(o =>
        {
            o.ApiUrl = ApiUrl;
            o.ApiKey = ApiKey;
            o.ApiSecret = ApiSecret;
        }).BuildServiceProvider().GetRequiredService<IPasswordlessClient>();
}