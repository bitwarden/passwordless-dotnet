using System;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests.Infra;

[Collection(TestApiFixture.Collection.Name)]
public abstract class ApiTestBase(TestApiFixture api, ITestOutputHelper testOutput) : IDisposable
{
    protected TestApiFixture Api { get; } = api;

    protected ITestOutputHelper TestOutput { get; } = testOutput;

    public void Dispose()
    {
        // Ideally we should route the logs in realtime, but it's a bit tedious
        // with the way the TestContainers library is designed.
        TestOutput.WriteLine(Api.GetLogs());
    }
}