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
    public async Task I_can_define_an_endpoint_that_will_create_a_registration_token()
    {
        // Arrange
        using var client = await App.CreateClientAsync();

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

        using var response = await client.SendAsync(request);

        var responseText = await response.Content.ReadAsStringAsync();
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseJson.GetProperty("token").GetString().Should().StartWith("register_");
    }
}