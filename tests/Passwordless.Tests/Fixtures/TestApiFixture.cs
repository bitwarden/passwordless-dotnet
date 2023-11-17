using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Xunit;

namespace Passwordless.Tests.Fixtures;

public class TestApiFixture : IAsyncLifetime
{
    private const string ManagementKey = "yourStrong(!)ManagementKey";
    private const string DatabaseHost = "database";
    private const ushort ApiPort = 8080;

    private readonly HttpClient _http = new();

    private readonly INetwork _network;
    private readonly MsSqlContainer _databaseContainer;
    private readonly IContainer _apiContainer;

    private readonly MemoryStream _databaseContainerStdOut = new();
    private readonly MemoryStream _databaseContainerStdErr = new();
    private readonly MemoryStream _apiContainerStdOut = new();
    private readonly MemoryStream _apiContainerStdErr = new();

    private string PublicApiUrl => $"http://{_apiContainer.Hostname}:{_apiContainer.GetMappedPublicPort(ApiPort)}";

    public TestApiFixture()
    {
        _network = new NetworkBuilder()
            .Build();

        _databaseContainer = new MsSqlBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases(DatabaseHost)
            .WithOutputConsumer(
                Consume.RedirectStdoutAndStderrToStream(_databaseContainerStdOut, _databaseContainerStdErr)
            )
            .Build();

        _apiContainer = new ContainerBuilder()
            // https://github.com/passwordless/passwordless-server/pkgs/container/passwordless-test-api
            // TODO: replace with ':stable' after the next release of the server.
            .WithImage("ghcr.io/passwordless/passwordless-test-api:latest")
            // Make sure we always have the latest version of the image
            .WithImagePullPolicy(PullPolicy.Always)
            .WithNetwork(_network)
            // Run in development environment to execute migrations
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment("ASPNETCORE_HTTP_PORTS", ApiPort.ToString())
            .WithEnvironment("ConnectionStrings__sqlite:api", "")
            .WithEnvironment("ConnectionStrings__mssql:api",
                $"Server={DatabaseHost},{MsSqlBuilder.MsSqlPort};" +
                "Database=Passwordless;" +
                $"User Id={MsSqlBuilder.DefaultUsername};" +
                $"Password={MsSqlBuilder.DefaultPassword};" +
                "Trust Server Certificate=true;" +
                "Trusted_Connection=false;"
            )
            .WithEnvironment("PasswordlessManagement__ManagementKey", ManagementKey)
            .WithPortBinding(ApiPort, true)
            // Wait until the API is launched, has performed migrations, and is ready to accept requests
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r
                    .ForPath("/")
                    .ForPort(ApiPort)
                    .ForStatusCode(HttpStatusCode.OK)
                )
            )
            .WithOutputConsumer(
                Consume.RedirectStdoutAndStderrToStream(_apiContainerStdOut, _apiContainerStdErr)
            )
            .Build();

        _http.DefaultRequestHeaders.Add("ManagementKey", ManagementKey);
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Introduce a timeout to avoid waiting forever for the containers to start
            // in case something goes wrong (e.g. wait strategy never succeeds).
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

            await _network.CreateAsync(timeoutCts.Token);
            await _databaseContainer.StartAsync(timeoutCts.Token);
            await _apiContainer.StartAsync(timeoutCts.Token);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            throw new OperationCanceledException(
                "Failed to start the containers within the allotted timeout. " +
                "This probably means that something went wrong during container initialization. " +
                "See the logs for more info." +
                Environment.NewLine + Environment.NewLine +
                GetLogs(),
                ex
            );
        }
    }

    public async Task<IPasswordlessClient> CreateClientAsync()
    {
        using var response = await _http.PostAsJsonAsync(
            $"{PublicApiUrl}/admin/apps/app{Guid.NewGuid():N}/create",
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

        var services = new ServiceCollection();

        services.AddPasswordlessSdk(options =>
        {
            options.ApiUrl = PublicApiUrl;
            options.ApiKey = apiKey;
            options.ApiSecret = apiSecret ??
                                throw new InvalidOperationException("Cannot extract API Secret from the response.");
        });

        return services.BuildServiceProvider().GetRequiredService<IPasswordlessClient>();
    }

    public string GetLogs()
    {
        var databaseContainerStdOutText = Encoding.UTF8.GetString(
            _databaseContainerStdOut.ToArray()
        );

        var databaseContainerStdErrText = Encoding.UTF8.GetString(
            _databaseContainerStdErr.ToArray()
        );

        var apiContainerStdOutText = Encoding.UTF8.GetString(
            _apiContainerStdOut.ToArray()
        );

        var apiContainerStdErrText = Encoding.UTF8.GetString(
            _apiContainerStdErr.ToArray()
        );

        // API logs are typically more relevant, so put them first
        return
            $"""
             # API container STDOUT:

             {apiContainerStdOutText}

             # API container STDERR:

             {apiContainerStdErrText}
             
             # Database container STDOUT:
             
             {databaseContainerStdOutText}
             
             # Database container STDERR:
             
             {databaseContainerStdErrText}
             """;
    }

    public async Task DisposeAsync()
    {
        await _apiContainer.DisposeAsync();
        await _databaseContainer.DisposeAsync();
        await _network.DisposeAsync();

        _databaseContainerStdOut.Dispose();
        _databaseContainerStdErr.Dispose();
        _apiContainerStdOut.Dispose();
        _apiContainerStdErr.Dispose();

        _http.Dispose();
    }
}