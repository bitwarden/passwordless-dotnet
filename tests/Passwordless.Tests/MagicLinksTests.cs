using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Passwordless.Models;
using Passwordless.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Passwordless.Tests;

public class MagicLinksTests(TestApiFixture api, ITestOutputHelper testOutput) : ApiTestBase(api, testOutput)
{
    [Fact]
    public async Task I_can_send_a_magic_link()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();
        var request = new SendMagicLinkRequest("passwordless@dev.test", "https://www.example.com?token=__TOKEN__", "user");
        
        // Act
        var action = async () => await passwordless.SendMagicLinkAsync(request, CancellationToken.None);
        
        // Assert
        await action.Should().NotThrowAsync();
    }
}