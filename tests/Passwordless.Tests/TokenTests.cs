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
        var ex = await Assert.ThrowsAnyAsync<PasswordlessApiException>(async () =>
            await passwordless.VerifyTokenAsync("invalid")
        );

        ex.Details.Status.Should().Be(400);
        ex.Details.Title.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task I_can_try_to_verify_a_tampered_signin_token_and_get_an_error()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act & assert
        var ex = await Assert.ThrowsAnyAsync<PasswordlessApiException>(async () =>
            await passwordless.VerifyTokenAsync("verify_something_that_looks_like_a_token_but_is_not")
        );

        ex.Details.Status.Should().Be(400);
        ex.Details.Title.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task I_can_try_to_verify_an_invalid_signin_token_and_get_an_error()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();

        // Act & assert
        var ex = await Assert.ThrowsAnyAsync<PasswordlessApiException>(async () =>
            await passwordless.VerifyTokenAsync(
                "verify_" +
                "k8Qg4kXVl8D2aunn__jMT7td5endUueS9zEG8zIsu0lqQjfFAQXcABPX_wlDNbBlTNiB2SQ5MjQ0ZmUzYS0wOGExLTRlMTctOTMwZS1i" +
                "YWZhNmM0OWJiOGWucGFzc2tleV9zaWduaW7AwMDAwMDA2SQ3NGUxMzFjOS0yNDZhLTRmNzYtYjIxMS1jNzBkZWQ1Mjg2YzLX_wlDJIBl" +
                "TNgJv2FkbWluY29uc29sZTAxLmxlc3NwYXNzd29yZC5kZXbZJ2h0dHBzOi8vYWRtaW5jb25zb2xlMDEubGVzc3Bhc3N3b3JkLmRldsOy" +
                "Q2hyb21lLCBXaW5kb3dzIDEwolVBqXRlc3Rlc3RzZcQghR4WgXh0HvbrT27GvP0Pkk4HmfL2b0ucVVSRlDElp_fOeb02NQ"
            )
        );

        ex.Details.Status.Should().Be(400);
        ex.Details.Title.Should().NotBeNullOrWhiteSpace();
    }
}