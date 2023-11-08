using Passwordless.Tests.Fixtures;
using Passwordless.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests;

public class Tests : ApiTestBase
{
    public Tests(TestApiFixture api, ITestOutputHelper testOutput)
        : base(api, testOutput)
    {
    }

    [Fact]
    public async Task Test()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act
        var users = await passwordless.ListUsersAsync();

        // Assert
        Assert.Empty(users);
    }
}