using FluentAssertions;
using Passwordless.Tests.Fixtures;
using Passwordless.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests;

public class UserTests : ApiTestBase
{
    public UserTests(TestApiFixture api, ITestOutputHelper testOutput)
        : base(api, testOutput)
    {
    }

    [Fact]
    public async Task I_can_list_users_and_get_an_empty_collection_if_there_are_not_any()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act
        var userCountResponse = await passwordless.GetUsersCountAsync();
        var users = await passwordless.ListUsersAsync();

        // Assert
        userCountResponse.Count.Should().Be(users.Count);
        users.Should().BeEmpty();
    }
}