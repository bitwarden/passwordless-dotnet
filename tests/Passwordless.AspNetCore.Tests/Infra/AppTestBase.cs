using System;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.AspNetCore.Tests.Infra;

[Collection(TestAppFixture.Collection.Name)]
public abstract class AppTestBase : IDisposable
{
    protected TestAppFixture App { get; }

    protected ITestOutputHelper TestOutput { get; }

    protected AppTestBase(TestAppFixture app, ITestOutputHelper testOutput)
    {
        App = app;
        TestOutput = testOutput;
    }

    public void Dispose()
    {
        // Ideally we should route the logs in realtime, but it's a bit tedious
        // with the way the TestContainers library is designed.
        TestOutput.WriteLine(App.GetLogs());
    }
}