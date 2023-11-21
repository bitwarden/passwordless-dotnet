using System.Threading.Tasks;
using Passwordless.Tests.Infra;
using Xunit;

namespace Passwordless.AspNetCore.Tests.Infra;

// xUnit can't initialize fixture from another assembly, so we have to wrap it
public class TestApiFixture : TestApi, IAsyncLifetime
{
    async Task IAsyncLifetime.DisposeAsync() => await base.DisposeAsync();

    [CollectionDefinition(Name)]
    public class Collection : ICollectionFixture<TestApiFixture>
    {
        public const string Name = "TestApiFixtureCollection";
    }
}