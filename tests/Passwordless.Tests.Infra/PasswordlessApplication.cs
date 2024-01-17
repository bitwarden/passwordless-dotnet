using Microsoft.Extensions.DependencyInjection;

namespace Passwordless.Tests.Infra;

public record PasswordlessApplication(string Name, string ApiUrl, string ApiSecret, string ApiKey)
{
    public IPasswordlessClient CreateClient() =>
        // Initialize using a service container to cover more code paths in tests
        new ServiceCollection().AddPasswordlessSdk(o =>
        {
            o.ApiUrl = ApiUrl;
            o.ApiKey = ApiKey;
            o.ApiSecret = ApiSecret;
        }).BuildServiceProvider().GetRequiredService<IPasswordlessClient>();
}