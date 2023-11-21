using System;
using System.Globalization;
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
using Microsoft.Extensions.DependencyInjection;

namespace Passwordless.Tests.Infra;

public class TestApi : IAsyncDisposable
{
    private const string ManagementKey = "yourStrong(!)ManagementKey";
    private const ushort ApiPort = 8080;

    private readonly HttpClient _http = new();

    private readonly IContainer _apiContainer;
    private readonly MemoryStream _apiContainerStdOut = new();
    private readonly MemoryStream _apiContainerStdErr = new();

    private string PublicApiUrl => $"http://{_apiContainer.Hostname}:{_apiContainer.GetMappedPublicPort(ApiPort)}";

    public TestApi()
    {
        _apiContainer = new ContainerBuilder()
            // https://github.com/passwordless/passwordless-server/pkgs/container/passwordless-test-api
            .WithImage("ghcr.io/passwordless/passwordless-test-api:stable")
            // Make sure we always have the latest version of the image
            .WithImagePullPolicy(PullPolicy.Always)
            // Run in development environment to enable migrations
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            // Explicitly set the HTTP port to avoid relying on the default value
            .WithEnvironment("ASPNETCORE_HTTP_PORTS", ApiPort.ToString(CultureInfo.InvariantCulture))
            // We need the management key to create apps
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
            new { AdminEmail = "test@passwordless.dev" }
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
        var apiContainerStdOutText = Encoding.UTF8.GetString(
            _apiContainerStdOut.ToArray()
        );

        var apiContainerStdErrText = Encoding.UTF8.GetString(
            _apiContainerStdErr.ToArray()
        );

        return
            $"""
             # API container STDOUT:

             {apiContainerStdOutText}

             # API container STDERR:

             {apiContainerStdErrText}
             """;
    }

    public async ValueTask DisposeAsync()
    {
        await _apiContainer.DisposeAsync();
        _apiContainerStdOut.Dispose();
        _apiContainerStdErr.Dispose();

        _http.Dispose();
    }
}