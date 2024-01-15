using System;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.AspNetCore.Tests.Infra;

[Collection(TestAppFixture.Collection.Name)]
public abstract class AppTestBase(TestAppFixture app, ITestOutputHelper testOutput) : IDisposable
{
    protected TestAppFixture App { get; } = app;

    protected ITestOutputHelper TestOutput { get; } = testOutput;

    public void Dispose()
    {
        // Ideally we should route the logs in realtime, but it's a bit tedious
        // with the way the TestContainers library is designed.
        TestOutput.WriteLine(App.GetLogs());
    }
}