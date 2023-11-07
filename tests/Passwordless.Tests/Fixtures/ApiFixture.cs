using System.Net;
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
    private readonly HttpClient _http = new();

    private readonly INetwork _network;
    private readonly MsSqlContainer _databaseContainer;
    private readonly IContainer _apiContainer;

    private string ExposedApiUrl => $"http://localhost:{_apiContainer.GetMappedPublicPort(80)}";

    // TODO: route container logs to test output
    public ApiFixture()
    {
        const string ManagementKey = "yourStrong(!)ManagementKey";

        _network = new NetworkBuilder()
            .Build();

        _databaseContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithNetwork(_network)
            .WithNetworkAliases("database")
            .Build();

        _apiContainer = new ContainerBuilder()
            // https://github.com/passwordless/passwordless-server/pkgs/container/api-test-server
            // Note: replace with ':stable' after the next release of the server.
            .WithImage("ghcr.io/passwordless/api-test-server:latest")
            .WithNetwork(_network)
            // Run in development environment to execute migrations
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("ConnectionStrings__sqlite:api", "")
            .WithEnvironment("ConnectionStrings__mssql:api",
                $"Server=database,{MsSqlBuilder.MsSqlPort};" +
                $"Database=Passwordless;" +
                $"User Id={MsSqlBuilder.DefaultUsername};" +
                $"Password={MsSqlBuilder.DefaultPassword};" +
                "Trust Server Certificate=true;" +
                "Trusted_Connection=false;"
            )
            .WithEnvironment("PasswordlessManagement__ManagementKey", ManagementKey)
            .WithPortBinding(80, true)
            // Wait until the API is ready to accept requests and perform migrations at the same time
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r
                    .ForPath("/")
                    .ForStatusCode(HttpStatusCode.OK)))
            .Build();

        _http.DefaultRequestHeaders.Add("ManagementKey", ManagementKey);
    }

    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await _databaseContainer.StartAsync();
        await _apiContainer.StartAsync();
    }

    public async Task<PasswordlessClient> CreateClientAsync()
    {
        // Create an app in the API
        using var response = await _http.PostAsJsonAsync(
            $"{ExposedApiUrl}/admin/apps/app{Guid.NewGuid():N}/create",
            new { AdminEmail = "foo@bar.com", EventLoggingIsEnabled = true }
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Failed to create an app. " +
                $"Status code: {(int)response.StatusCode}. " +
                $"Response body: {await response.Content.ReadAsStringAsync()}."
            );
        }

        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        var apiKey = responseContent.GetProperty("apiKey1").GetString();
        var apiSecret = responseContent.GetProperty("apiSecret1").GetString();

        // Configure client
        var options = new PasswordlessOptions
        {
            ApiUrl = ExposedApiUrl,
            ApiKey = apiKey,
            ApiSecret = apiSecret ??
                        throw new InvalidOperationException("Cannot extract API Secret from the response.")
        };

        return new PasswordlessClient(_http, options);
    }

    public async Task DisposeAsync()
    {
        await _apiContainer.DisposeAsync();
        await _databaseContainer.DisposeAsync();
        await _network.DisposeAsync();

        _http.Dispose();
    }
}