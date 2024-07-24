using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Passwordless.Tests;

public class ServiceRegistrationTests
{
    [Fact]
    public async Task I_can_register_a_client_with_invalid_credentials_and_not_receive_an_error_until_it_is_used()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddPasswordlessSdk(o =>
            {
                o.ApiSecret = "";
                o.ApiKey = "";
            })
            .BuildServiceProvider();

        var passwordless = services.GetRequiredService<IPasswordlessClient>();

        // Act & assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            await passwordless.CreateRegisterTokenAsync(new RegisterOptions("user123", "User"))
        );
    }
}