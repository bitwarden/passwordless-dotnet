using Microsoft.AspNetCore.Mvc.Testing;
using Passwordless.Tests.Infra;

namespace Passwordless.AspNetCore.Tests.Infra;

public class TestApp : WebApplicationFactory<Dummy.Program>
{
    private readonly TestApi _api = new();
}