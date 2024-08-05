using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Passwordless.Tests;

public class ServiceRegistrationTests
{
    [Fact]
    public void I_can_register_a_client_with_options_derived_from_configuration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("Passwordless:ApiSecret", "foo"),
                new KeyValuePair<string, string?>("Passwordless:ApiKey", "bar")
            ])
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddPasswordlessSdk(configuration.GetSection("Passwordless"))
            .BuildServiceProvider();

        // Act
        var options = services.GetRequiredService<IOptions<PasswordlessOptions>>();

        // Assert
        options.Value.ApiSecret.Should().Be("foo");
        options.Value.ApiKey.Should().Be("bar");
    }

    [Fact]
    public void I_can_register_a_client_and_update_its_options_at_any_point_in_time()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("Passwordless:ApiSecret", "foo"),
                new KeyValuePair<string, string?>("Passwordless:ApiKey", "bar")
            ])
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddPasswordlessSdk(configuration.GetSection("Passwordless"))
            .BuildServiceProvider();

        // Act
        foreach (var provider in configuration.Providers)
        {
            provider.Set("Passwordless:ApiSecret", "baz");
            provider.Set("Passwordless:ApiKey", "zap");
        }

        // Assert
        var options = services.GetRequiredService<IOptions<PasswordlessOptions>>();
        options.Value.ApiSecret.Should().Be("baz");
        options.Value.ApiKey.Should().Be("zap");
    }

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

    [Fact]
    public void I_can_register_a_client_and_retrieve_a_new_copy_for_each_scope()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddPasswordlessSdk(o =>
            {
                o.ApiSecret = "foo";
                o.ApiKey = "bar";
            })
            .BuildServiceProvider();

        // Act
        using var scope1 = services.CreateScope();
        var client1 = scope1.ServiceProvider.GetRequiredService<IPasswordlessClient>();

        using var scope2 = services.CreateScope();
        var client2 = scope2.ServiceProvider.GetRequiredService<IPasswordlessClient>();

        // Assert
        client1.Should().NotBeSameAs(client2);
    }
}