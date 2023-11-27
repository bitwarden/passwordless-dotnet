using System.Collections.Generic;
using System.Linq;
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
    public async Task I_can_define_a_register_endpoint()
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

    [Fact]
    public async Task I_can_define_a_register_endpoint_and_it_will_reject_invalid_registration_attempts()
    {
        // Arrange
        using var http = await App.CreateClientAsync();

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "/register");
        request.Content = new StringContent(
            // lang=json
            """
            {
              "email": null
            }
            """,
            Encoding.UTF8,
            "application/json"
        );

        using var response = await http.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Figure out why this fails")]
    public async Task I_can_define_a_register_endpoint_and_it_will_reject_duplicate_registration_attempts()
    {
        // Arrange
        using var http = await App.CreateClientAsync();

        // Act
        var responses = new List<HttpResponseMessage>();
        for (var i = 0; i < 5; i++)
        {
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

            var response = await http.SendAsync(request);
            responses.Add(response);
        }

        // Assert
        responses.Take(1).Should().OnlyContain(r => r.StatusCode == HttpStatusCode.OK);
        responses.Skip(1).Should().OnlyContain(r => r.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Bug: this currently does not return 400 status code. Task: PAS-260")]
    public async Task I_can_define_a_signin_endpoint_and_it_will_reject_invalid_signin_attempts()
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

    // TODO: expand with more tests when magic links endpoint support is added
    // https://github.com/bitwarden/passwordless-dotnet/pull/75
}