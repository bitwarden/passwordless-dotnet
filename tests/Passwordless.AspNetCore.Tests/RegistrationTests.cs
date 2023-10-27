using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Passwordless.AspNetCore.Services;

namespace Passwordless.AspNetCore.Tests;

public class RegistrationTests
{
    [Fact]
    public void AllExpectedServicesAndOptionsResolve()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiSecret"] = "FakeApiSecret",
                ["ApiUrl"] = "https://example.org",
                ["SignInScheme"] = "Cookies",
                ["Register:Discoverable"] = "false"
            })
            .Build();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<TestDbContext>();

        serviceCollection
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<TestDbContext>()
            .AddPasswordless(configuration);

        var provider = serviceCollection.BuildServiceProvider();

        _ = provider.GetRequiredService<PasswordlessClient>();
        _ = provider.GetRequiredService<IPasswordlessService<PasswordlessRegisterRequest>>();
        var optionsAccessor = provider.GetRequiredService<IOptions<PasswordlessAspNetCoreOptions>>();
        var options = optionsAccessor.Value;
        Assert.Equal("FakeApiSecret", options.ApiSecret);
        Assert.Equal("https://example.org", options.ApiUrl);
        Assert.Equal("Cookies", options.SignInScheme);
        Assert.False(options.Register.Discoverable);
    }
}

public class TestDbContext : IdentityDbContext<IdentityUser>
{

}