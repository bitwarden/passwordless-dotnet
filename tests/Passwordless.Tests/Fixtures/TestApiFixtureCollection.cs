using Xunit;

namespace Passwordless.Tests.Fixtures;

[CollectionDefinition(nameof(TestApiFixtureCollection))]
public class TestApiFixtureCollection : ICollectionFixture<TestApiFixture>
{
}