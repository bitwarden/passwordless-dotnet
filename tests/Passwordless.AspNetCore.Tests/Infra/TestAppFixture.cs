using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Passwordless.AspNetCore.Tests.Dummy;
using Passwordless.Tests.Infra;
using Xunit;

namespace Passwordless.AspNetCore.Tests.Infra;

// xUnit can't initialize a fixture from another assembly, so we have to wrap it
public partial class TestAppFixture : IAsyncLifetime
{
    private readonly TestApi _api = new();

    public async Task InitializeAsync() => await _api.InitializeAsync();

    public async Task<HttpClient> CreateClientAsync(Action<IServiceCollection>? configure = null)
    {
        var app = await _api.CreateAppAsync();

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(c => c
                .ConfigureTestServices(s =>
                {
                    s.Configure<PasswordlessAspNetCoreOptions>(o =>
                    {
                        o.ApiUrl = app.ApiUrl;
                        o.ApiSecret = app.ApiSecret;
                        o.ApiKey = app.ApiKey;
                    });

                    configure?.Invoke(s);
                })
            ).CreateClient();
    }

    public string GetLogs() => _api.GetLogs();

    public async Task DisposeAsync()
    {
        await _api.DisposeAsync();
    }
}

public partial class TestAppFixture
{
    [CollectionDefinition(Name)]
    public class Collection : ICollectionFixture<TestAppFixture>
    {
        public const string Name = nameof(TestAppFixture);
    }
}