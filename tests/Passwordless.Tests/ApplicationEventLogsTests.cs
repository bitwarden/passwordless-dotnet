using System.Threading.Tasks;
using FluentAssertions;
using Passwordless.Models;
using Passwordless.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests;

public class ApplicationEventLogsTests(TestApiFixture api, ITestOutputHelper testOutput) : ApiTestBase(api, testOutput)
{
    [Fact]
    public async Task I_can_view_application_event_logs_when_event_logs_are_enabled()
    {
        // Arrange
        var applicationName = TestApi.GetAppName();
        var passwordless = await Api.CreateClientAsync(applicationName);
        await Api.EnableEventLogsAsync(applicationName);


        // Act
        var response = await passwordless.GetEventLogAsync(new GetEventLogRequest { PageNumber = 1, NumberOfResults = 100 });

        // Assert
        response.Should().NotBeNull();
        response.TenantId.Should().Be(applicationName);
    }
}