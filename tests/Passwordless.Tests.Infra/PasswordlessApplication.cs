namespace Passwordless.Tests.Infra;

public record PasswordlessApplication(string Name, string ApiUrl, string ApiSecret, string ApiKey)
{
    public IPasswordlessClient CreateClient() => new PasswordlessClient(new PasswordlessOptions
    {
        ApiUrl = ApiUrl,
        ApiSecret = ApiSecret,
        ApiKey = ApiKey
    });
}