using System.Threading.Tasks;
using FluentAssertions;
using Passwordless.Models;
using Passwordless.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests;

public class ApplicationEventLogsTests : ApiTestBase
{
    public ApplicationEventLogsTests(TestApiFixture api, ITestOutputHelper testOutput) : base(api, testOutput)
    {
    }

    [Fact]
    public async Task I_can_view_application_event_logs_when_event_logs_are_enabled()
    {
        // Arrange
        const string applicationName = "testeventlogs";

        var passwordless = await Api.CreateClientAsync(applicationName);

        var response = await passwordless.GetEventLogAsync(new GetEventLogRequest { PageNumber = 1, NumberOfResults = 100 });

        response.Should().NotBeNull();
        response.ApplicationName.Should().Be(applicationName);
    }
}