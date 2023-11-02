using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.MsSql;
using Xunit;

namespace Passwordless.Tests.Fixtures;

public class ApiFixture : IAsyncLifetime
{
    private const string DatabasePassword = "Password1!";
    private const string ManagementKey = "FooBar";

    private readonly HttpClient _http = new();

    public INetwork Network { get; }
    public IContainer MssqlContainer { get; }
    public IContainer ApiContainer { get; }

    public ApiFixture()
    {
        Network = new NetworkBuilder()
            .WithName("Passwordless.NET.Tests")
            .Build();

        MssqlContainer = new MsSqlBuilder()
            .WithPassword(DatabasePassword)
            .WithNetwork(Network)
            .Build();

        ApiContainer = new ContainerBuilder()
            .WithImage("api") // temp
            .WithNetwork(Network)
            .WithEnvironment("ConnectionStrings__sqlite:api", "")
            .WithEnvironment("ConnectionStrings__mssql:api",
                $"Server={MssqlContainer.Hostname},1433;" +
                $"Database=passwordless_dev;" +
                $"User Id=sa;" +
                $"Password=${DatabasePassword};"
            )
            .WithEnvironment("PasswordlessManagement__ManagementKey", ManagementKey)
            .WithPortBinding(80, true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Network.CreateAsync();
        await MssqlContainer.StartAsync();
        await ApiContainer.StartAsync();
    }

    public async Task<PasswordlessClient> CreateClientAsync()
    {
        // Create an app in the API
        var apiUrl = $"http://localhost:{ApiContainer.GetMappedPublicPort(80)}";

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/admin/apps/{Guid.NewGuid():N}/create");
        request.Content = JsonContent.Create(new { AdminEmail = "foo@bar.com", EventLoggingIsEnabled = true });
        request.Headers.Add("ManagementKey", ManagementKey);

        using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Failed to create an app. Response body: " +
                await response.Content.ReadAsStringAsync()
            );
        }

        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        var apiKey = responseContent.GetProperty("ApiKey1").GetString();
        var apiSecret = responseContent.GetProperty("ApiSecret1").GetString();

        // Configure client
        var options = new PasswordlessOptions
        {
            ApiUrl = apiUrl,
            ApiKey = apiKey,
            ApiSecret = apiSecret ??
                        throw new InvalidOperationException("Cannot extract API Secret from the response.")
        };

        return new PasswordlessClient(_http, options);
    }

    public async Task DisposeAsync()
    {
        await ApiContainer.StopAsync();
        await MssqlContainer.StopAsync();
        await Network.DeleteAsync();

        await ApiContainer.DisposeAsync();
        await MssqlContainer.DisposeAsync();
        await Network.DisposeAsync();

        _http.Dispose();
    }
}
