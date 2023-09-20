using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;


namespace Passwordless.Tests;

public class PasswordlessClientTests
{
    private readonly PasswordlessClient _sut;

    public PasswordlessClientTests()
    {
        var services = new ServiceCollection();

        services.AddPasswordlessSdk(options =>
        {
            options.ApiUrl = "https://localhost:7002";
            options.ApiSecret = "test:secret:a679563b331846c79c20b114a4f56d02";
        });

        var provider = services.BuildServiceProvider();

        _sut = (PasswordlessClient)provider.GetRequiredService<IPasswordlessClient>();
    }

    [ApiFact]
    public async Task CreateRegisterTokenAsync_ThrowsExceptionWhenBad()
    {
        var exception = await Assert.ThrowsAnyAsync<HttpRequestException>(
            async () => await _sut.CreateRegisterTokenAsync(new RegisterOptions(null!, null!)));
    }

    [ApiFact]
    public async Task VerifyTokenAsync_DoesNotThrowOnBadToken()
    {
        var verifiedUser = await _sut.VerifyTokenAsync("bad_token");

        Assert.Null(verifiedUser);
    }

    [ApiFact]
    public async Task DeleteUserAsync_BadUserId_ThrowsException()
    {
        var exception = await Assert.ThrowsAnyAsync<HttpRequestException>(
            async () => await _sut.DeleteUserAsync(null!));
    }

    [ApiFact]
    public async Task ListAsiasesAsync_BadUserId_ThrowsException()
    {
        var exception = await Assert.ThrowsAnyAsync<HttpRequestException>(
            async () => await _sut.ListAliasesAsync(null!));
    }

    [ApiFact]
    public async Task ListCredentialsAsync_BadUserId_ThrowsException()
    {
        var exception = await Assert.ThrowsAnyAsync<PasswordlessApiException>(
            async () => await _sut.ListCredentialsAsync(null!));

        var errorCode = Assert.Contains("errorCode", (IDictionary<string, JsonElement>)exception.Details.Extensions);
        Assert.Equal(JsonValueKind.String, errorCode.ValueKind);
        Assert.Equal("missing_userid", errorCode.GetString());
    }

    [ApiFact]
    public async Task CreateRegisterTokenAsync_Works()
    {
        var userId = Guid.NewGuid().ToString();

        var response = await _sut.CreateRegisterTokenAsync(new RegisterOptions(userId, "test_username"));

        Assert.NotNull(response.Token);
        Assert.StartsWith("register_", response.Token);
    }

    [ApiFact]
    public async Task VerifyTokenAsync_Works()
    {
        var user = await _sut.VerifyTokenAsync("verify_valid");

        Assert.NotNull(user);
        Assert.True(user.Success);
    }

    [ApiFact]
    public async Task ListUsersAsync_Works()
    {
        var users = await _sut.ListUsersAsync();

        Assert.NotEmpty(users);
    }

    [ApiFact]
    public async Task ListAliasesAsync_Works()
    {
        // Act
        var aliases = await _sut.ListAliasesAsync("has_aliases");

        // Assert
        Assert.NotEmpty(aliases);
    }

    [ApiFact]
    public async Task ListCredentialsAsync_Works()
    {
        var credentials = await _sut.ListCredentialsAsync("has_credentials");

        Assert.NotEmpty(credentials);
    }

    [ApiFact]
    public async Task DeleteCredentialAsync_Works()
    {
        await _sut.DeleteCredentialAsync("can_delete");
    }

    [ApiFact]
    public async Task GetUsersCountAsync_Works()
    {
        var usersCount = await _sut.GetUsersCountAsync();
        Assert.NotEqual(0, usersCount.Count);
    }
}