using Passwordless.Tests.Fixtures;
using Xunit;

namespace Passwordless.Tests;

public class Tests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture _api;

    public Tests(ApiFixture api)
    {
        _api = api;
    }

    [Fact]
    public async Task Test()
    {
        // Arrange
        var passwordless = await _api.CreateClientAsync();

        // Act
        var users = await passwordless.ListUsersAsync();

        // Assert
        Assert.Empty(users);
    }
}