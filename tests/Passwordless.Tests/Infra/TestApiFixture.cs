using System.Threading.Tasks;
using Xunit;

namespace Passwordless.Tests.Infra;

// xUnit can't initialize a fixture from another assembly, so we have to wrap it
public partial class TestApiFixture : TestApi, IAsyncLifetime
{
    async Task IAsyncLifetime.DisposeAsync() => await base.DisposeAsync();
}

public partial class TestApiFixture
{
    [CollectionDefinition(Name)]
    public class Collection : ICollectionFixture<TestApiFixture>
    {
        public const string Name = nameof(TestApiFixture);
    }
}