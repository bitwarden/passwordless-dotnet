using System;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.AspNetCore.Tests.Infra;

[Collection(TestApiFixture.Collection.Name)]
public abstract class ApiTestBase : IDisposable
{
    protected TestApiFixture Api { get; }

    protected ITestOutputHelper TestOutput { get; }

    protected ApiTestBase(TestApiFixture api, ITestOutputHelper testOutput)
    {
        Api = api;
        TestOutput = testOutput;
    }

    public void Dispose()
    {
        // Ideally we should route the logs in realtime, but it's a bit tedious
        // with the way the TestContainers library is designed.
        TestOutput.WriteLine(Api.GetLogs());
    }
}