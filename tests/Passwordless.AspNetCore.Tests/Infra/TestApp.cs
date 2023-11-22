using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Passwordless.Tests.Infra;

namespace Passwordless.AspNetCore.Tests.Infra;

public class TestApp : WebApplicationFactory<Dummy.Program>
{
    private readonly TestApi _api = new();

    public async Task InitializeAsync()
    {
        await _api.InitializeAsync();
    }
}