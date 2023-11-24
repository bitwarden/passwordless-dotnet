using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Passwordless.AspNetCore.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.AspNetCore.Tests;

public class EndpointTests : AppTestBase
{
    public EndpointTests(TestAppFixture app, ITestOutputHelper testOutput)
        : base(app, testOutput)
    {
    }

    [Fact]
    public async Task I_can_define_an_endpoint_to_create_a_register_token()
    {
        // Arrange
        using var http = await App.CreateClientAsync();

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "/register");
        request.Content = new StringContent(
            // lang=json
            """
            {
              "email": "test@passwordless.dev",
              "username": "test",
              "displayName": "Test User"
            }
            """,
            Encoding.UTF8,
            "application/json"
        );

        using var response = await http.SendAsync(request);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseJson.GetProperty("token").GetString().Should().StartWith("register_");
    }

    [Fact(Skip = "Bug: this currently does not return 400 status code")]
    public async Task I_can_define_an_endpoint_to_verify_a_signin_token_and_return_an_error_if_it_is_invalid()
    {
        // Arrange
        using var http = await App.CreateClientAsync();

        const string token =
            "verify_" +
            "k8Qg4kXVl8D2aunn__jMT7td5endUueS9zEG8zIsu0lqQjfFAQXcABPX_wlDNbBlTNiB2SQ5MjQ0ZmUzYS0wOGExLTRlMTctOTMwZS1i" +
            "YWZhNmM0OWJiOGWucGFzc2tleV9zaWduaW7AwMDAwMDA2SQ3NGUxMzFjOS0yNDZhLTRmNzYtYjIxMS1jNzBkZWQ1Mjg2YzLX_wlDJIBl" +
            "TNgJv2FkbWluY29uc29sZTAxLmxlc3NwYXNzd29yZC5kZXbZJ2h0dHBzOi8vYWRtaW5jb25zb2xlMDEubGVzc3Bhc3N3b3JkLmRldsOy" +
            "Q2hyb21lLCBXaW5kb3dzIDEwolVBqXRlc3Rlc3RzZcQghR4WgXh0HvbrT27GvP0Pkk4HmfL2b0ucVVSRlDElp_fOeb02NQ";

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "/login");
        request.Content = new StringContent(
            // lang=json
            $$"""
            {
              "token": "{{token}}"
            }
            """,
            Encoding.UTF8,
            "application/json"
        );

        using var response = await http.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}