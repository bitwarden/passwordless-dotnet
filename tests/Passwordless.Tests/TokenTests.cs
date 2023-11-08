using FluentAssertions;
using Passwordless.Tests.Fixtures;
using Passwordless.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests;

public class TokenTests : ApiTestBase
{
    public TokenTests(TestApiFixture api, ITestOutputHelper testOutput)
        : base(api, testOutput)
    {
    }

    [Fact]
    public async Task I_can_create_a_register_token()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act
        var response = await passwordless.CreateRegisterTokenAsync(
            new RegisterOptions("user123", "John Doe")
        );

        // Assert
        response.Token.Should().NotBeNullOrWhiteSpace();
        response.Token.Should().StartWith("register_");
    }

    [Fact]
    public async Task I_can_try_to_verify_a_poorly_formatted_signin_token_and_get_an_error()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act & assert
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await passwordless.VerifyTokenAsync("invalid")
        );
    }

    [Fact(Skip = "Need to figure out a syntactically correct token that is invalid")]
    public async Task I_can_try_to_verify_an_invalid_signin_token_and_get_a_null_response()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act
        var response = await passwordless.VerifyTokenAsync("verify_foobar");

        // Assert
        response.Should().BeNull();
    }
}