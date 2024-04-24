using System;
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
    public async Task I_can_send_a_magic_link_with_a_specified_time_to_live()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();
        var request = new SendMagicLinkRequest("test@passwordless.dev", "https://www.example.com?token=$TOKEN", "user", new TimeSpan(0, 15, 0));

        // Act
        var action = async () => await passwordless.SendMagicLinkAsync(request, CancellationToken.None);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task I_can_send_a_magic_link_without_a_time_to_live()
    {
        // Arrange
        var passwordless = await Api.CreateClientAsync();
        var request = new SendMagicLinkRequest("test@passwordless.dev", "https://www.example.com?token=$TOKEN", "user", null);

        // Act
        var action = async () => await passwordless.SendMagicLinkAsync(request, CancellationToken.None);

        // Assert
        await action.Should().NotThrowAsync();
    }
}