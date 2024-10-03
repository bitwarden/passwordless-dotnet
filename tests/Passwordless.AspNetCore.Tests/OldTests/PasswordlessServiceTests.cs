using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Passwordless.AspNetCore.Services;
using Passwordless.AspNetCore.Services.Implementations;
using Passwordless.Models;
using Xunit;
using AuthenticationOptions = Microsoft.AspNetCore.Authentication.AuthenticationOptions;

namespace Passwordless.AspNetCore.Tests.OldTests;

public class PasswordlessServiceTests
{
    private readonly Mock<IPasswordlessClient> _mockPasswordlessClient = new();
    private readonly TestUserStore _testUserStore = new();
    private readonly PasswordlessAspNetCoreOptions _options = new() { ApiSecret = "FooBar" };
    private readonly Mock<IUserClaimsPrincipalFactory<TestUser>> _mockUserClaimsPrincipalFactory = new();
    private readonly Mock<ICustomizeRegisterOptions> _mockCustomizeRegisterOptions = new();
    private readonly Mock<IServiceProvider> _mockServiceProvider = new();
    private readonly Fixture _fixture = new();

    public PasswordlessServiceTests()
    {
        _fixture.Register(() => new VerifiedUser(
            Guid.NewGuid().ToString(),
            _fixture.Create<byte[]>(),
            true,
            _fixture.Create<DateTime>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<DateTime>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<string>()));
    }


    private PasswordlessService<TestUser> CreateSut()
    {
        var mockOptions = new Mock<IOptions<PasswordlessAspNetCoreOptions>>();
        mockOptions.Setup(o => o.Value)
            .Returns(_options);

        return new PasswordlessService<TestUser>(
            _mockPasswordlessClient.Object,
            _testUserStore,
            NullLogger<PasswordlessService<TestUser>>.Instance,
            mockOptions.Object,
            _mockUserClaimsPrincipalFactory.Object,
            _mockCustomizeRegisterOptions.Object,
            _mockServiceProvider.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_Works()
    {
        var expectedResponse = new RegisterTokenResponse("test_token");

        _options.Register.Discoverable = true;

        _mockPasswordlessClient
            .Setup(s => s.CreateRegisterTokenAsync(
                It.Is<RegisterOptions>(o => o.UserId != null && o.Discoverable == true), default))
            .ReturnsAsync(expectedResponse);

        var sut = CreateSut();

        var result = await sut.RegisterUserAsync(new PasswordlessRegisterRequest(
            "test_username",
            "Test User",
            new HashSet<string> { "test" }), CancellationToken.None);

        var okResult = Assert.IsAssignableFrom<Ok<RegisterTokenResponse>>(result);
        Assert.Equal(expectedResponse, okResult.Value);

        Assert.Single(_testUserStore.InnerStore, kvp => kvp.Value.Username == "test_username");
    }

    [Fact]
    public async Task RegisterUserAsync_CustomizeOptionsReturnsNull_ReturnsUnauthorized()
    {
        _mockCustomizeRegisterOptions
            .Setup(s => s.CustomizeAsync(It.IsAny<CustomizeRegisterOptionsContext>(), CancellationToken.None))
            .Callback<CustomizeRegisterOptionsContext, CancellationToken>((context, _) => context.Options = null);

        var sut = CreateSut();

        var result = await sut.RegisterUserAsync(new PasswordlessRegisterRequest(
            "test_username",
            "Test User",
            new HashSet<string> { "test" }), CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task RegisterUserAsync_CanNotCreateUser_ReturnsValidationProblems()
    {
        var mockUserStore = new Mock<IUserStore<TestUser>>();

        mockUserStore
            .Setup(s => s.SetUserNameAsync(It.IsAny<TestUser>(), "test_username", CancellationToken.None))
            .Callback<TestUser, string, CancellationToken>(
                (user, username, token) => user.Username = username);

        mockUserStore
            .Setup(s => s.CreateAsync(It.IsAny<TestUser>(), CancellationToken.None))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = "failed",
                Description = "Failed to create user",
            }));

        // Have to manually create the sut in this test because we want a mocked IUserStore
        var sut = new PasswordlessService<TestUser>(
            _mockPasswordlessClient.Object,
            mockUserStore.Object,
            NullLogger<PasswordlessService<TestUser>>.Instance,
            Options.Create(_options),
            _mockUserClaimsPrincipalFactory.Object,
            _mockCustomizeRegisterOptions.Object,
            _mockServiceProvider.Object);

        var result = await sut.RegisterUserAsync(
            new PasswordlessRegisterRequest("test_username", "Test User", new HashSet<string> { "" }),
            CancellationToken.None);

        var problemResult = Assert.IsAssignableFrom<ProblemHttpResult>(result);
        var validationProblems = Assert.IsAssignableFrom<HttpValidationProblemDetails>(problemResult.ProblemDetails);
        Assert.True(validationProblems.Errors.TryGetValue("failed", out var failedErrors));
        var failedError = Assert.Single(failedErrors);
        Assert.Equal("Failed to create user", failedError);
    }

    [Fact]
    public async Task RegisterUserAsync_AppSetupForEmail_Works()
    {
        var identityOptions = new IdentityOptions();
        identityOptions.User.RequireUniqueEmail = true;

        _mockServiceProvider
            .Setup(s => s.GetService(typeof(IOptions<IdentityOptions>)))
            .Returns(Options.Create(identityOptions));

        var sut = CreateSut();

        var result = await sut.RegisterUserAsync(new PasswordlessRegisterRequest("my_email_username", "My Email Key", null)
        {
            Email = "my_email@email.com",
        }, CancellationToken.None);

        var createdUser = await _testUserStore.FindByNameAsync("my_email_username", CancellationToken.None);
        Assert.NotNull(createdUser);
        Assert.Equal("my_email@email.com", createdUser.Email);
    }

    [Fact]
    public async Task RegisterUserAsync_AppSetupForEmail_NullEmailProvided_ReturnsValidationProblem()
    {
        var identityOptions = new IdentityOptions();
        identityOptions.User.RequireUniqueEmail = true;

        _mockServiceProvider
            .Setup(s => s.GetService(typeof(IOptions<IdentityOptions>)))
            .Returns(Options.Create(identityOptions));

        var sut = CreateSut();

        var result = await sut.RegisterUserAsync(new PasswordlessRegisterRequest("my_email_username", "My Email Key", null)
        {
            Email = null!,
        }, CancellationToken.None);

        var problemResult = Assert.IsAssignableFrom<ProblemHttpResult>(result);
        var validationProblem = Assert.IsAssignableFrom<HttpValidationProblemDetails>(problemResult.ProblemDetails);
        Assert.True(validationProblem.Errors.TryGetValue("invalid_email", out var errors));
        Assert.Single(errors);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(Guid? userId, string? authenticationType = "test")
    {
        var claims = new List<Claim>();
        if (userId.HasValue)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
        }
        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
    }

    [Fact]
    public async Task LoginUserAsync_UsesDefaultSchemeIfNoneSpecified()
    {
        var verifiedUser = _fixture.Create<VerifiedUser>();

        var user = new TestUser
        {
            Id = Guid.Parse(verifiedUser.UserId),
            Username = "login_test_user",
        };

        await _testUserStore.CreateAsync(user);

        _mockPasswordlessClient
            .Setup(s => s.VerifyAuthenticationTokenAsync("test_token", default))
            .ReturnsAsync(verifiedUser);

        _mockUserClaimsPrincipalFactory
            .Setup(s => s.CreateAsync(user))
            .ReturnsAsync(CreateClaimsPrincipal(user.Id));

        var sut = CreateSut();

        var result = await sut.LoginUserAsync(new PasswordlessLoginRequest("test_token"), CancellationToken.None);
        var signInResult = Assert.IsAssignableFrom<SignInHttpResult>(result);

        Assert.Null(signInResult.AuthenticationScheme);
    }

    [Fact]
    public async Task LoginUserAsync_UsesOurOptionIfSpecified()
    {

        var verifiedUser = _fixture.Create<VerifiedUser>();

        var user = new TestUser
        {
            Id = Guid.Parse(verifiedUser.UserId),
            Username = "login_test_user_2",
        };

        _options.SignInScheme = "our_scheme";

        await _testUserStore.CreateAsync(user);

        _mockPasswordlessClient
            .Setup(s => s.VerifyAuthenticationTokenAsync("test_token", default))
            .ReturnsAsync(verifiedUser);

        _mockUserClaimsPrincipalFactory
            .Setup(s => s.CreateAsync(user))
            .ReturnsAsync(CreateClaimsPrincipal(user.Id));

        var sut = CreateSut();

        var result = await sut.LoginUserAsync(new PasswordlessLoginRequest("test_token"), CancellationToken.None);
        var signInResult = Assert.IsAssignableFrom<SignInHttpResult>(result);

        Assert.Equal("our_scheme", signInResult.AuthenticationScheme);
    }

    [Fact]
    public async Task LoginUserAsync_TriesAuthenticationOptionsIfOursIsNull()
    {

        var verifiedUser = _fixture.Create<VerifiedUser>();

        var user = new TestUser
        {
            Id = Guid.Parse(verifiedUser.UserId),
            Username = "login_test_user_3",
        };

        _options.SignInScheme = null;

        _mockServiceProvider
            .Setup(s => s.GetService(typeof(IOptions<AuthenticationOptions>)))
            .Returns(Options.Create(new AuthenticationOptions
            {
                DefaultSignInScheme = "auth_options_scheme",
            }));

        await _testUserStore.CreateAsync(user);

        _mockPasswordlessClient
            .Setup(s => s.VerifyAuthenticationTokenAsync("test_token", default))
            .ReturnsAsync(verifiedUser);

        _mockUserClaimsPrincipalFactory
            .Setup(s => s.CreateAsync(user))
            .ReturnsAsync(CreateClaimsPrincipal(user.Id));

        var sut = CreateSut();

        var result = await sut.LoginUserAsync(new PasswordlessLoginRequest("test_token"), CancellationToken.None);
        var signInResult = Assert.IsAssignableFrom<SignInHttpResult>(result);

        Assert.Equal("auth_options_scheme", signInResult.AuthenticationScheme);
    }

    [Fact]
    public async Task LoginUserAsync_UserDoesNotExist_ReturnsUnauthorized()
    {

        var verifiedUser = _fixture.Create<VerifiedUser>();

        _mockPasswordlessClient
            .Setup(s => s.VerifyAuthenticationTokenAsync("test_token", default))
            .ReturnsAsync(verifiedUser);

        var sut = CreateSut();

        var result = await sut.LoginUserAsync(new PasswordlessLoginRequest("test_token"), CancellationToken.None);
        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task AddCredentialAsync_ReturnsRegisterTokenResponse()
    {
        var verifiedUser = _fixture.Create<VerifiedUser>();

        var user = new TestUser
        {
            Id = Guid.Parse(verifiedUser.UserId),
            Username = "add_credential_test_1",
        };

        await _testUserStore.CreateAsync(user);

        _mockPasswordlessClient
            .Setup(s => s.CreateRegisterTokenAsync(
                It.Is<RegisterOptions>(o => o.UserId == user.Id.ToString()
                    && o.Username == "add_credential_test_1"), default))
            .ReturnsAsync(new RegisterTokenResponse("test_register_token"));

        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(user.Id),
            CancellationToken.None);

        var okResult = Assert.IsAssignableFrom<Ok<RegisterTokenResponse>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("test_register_token", okResult.Value.Token);
    }

    [Fact]
    public async Task AddCredentialAsync_NullUsername_ReturnsUnauthorized()
    {
        var user = new TestUser
        {
            Id = Guid.NewGuid(),
            Username = null!,
        };

        await _testUserStore.CreateAsync(user);

        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(user.Id),
            CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task AddCredentialAsync_AppRequiresEmail_CreatesOptionsWithEmailAlias()
    {
        var user = new TestUser
        {
            Id = Guid.NewGuid(),
            Username = "username_with_email",
            Email = "test@email.com",
        };

        await _testUserStore.CreateAsync(user);

        var identityOptions = new IdentityOptions();
        identityOptions.User.RequireUniqueEmail = true;

        _mockServiceProvider
            .Setup(s => s.GetService(typeof(IOptions<IdentityOptions>)))
            .Returns(Options.Create(identityOptions));

        _mockPasswordlessClient.Setup(s => s.CreateRegisterTokenAsync(It.Is<RegisterOptions>(o
            => o.UserId == user.Id.ToString() && o.Username == "username_with_email" && o.Aliases != null && o.Aliases.Contains("test@email.com")), default))
            .ReturnsAsync(new RegisterTokenResponse("test_email_token"));

        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(user.Id),
            CancellationToken.None);

        var okResult = Assert.IsAssignableFrom<Ok<RegisterTokenResponse>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("test_email_token", okResult.Value.Token);
    }

    [Fact]
    public async Task AddCredentialAsync_CustomizeReturnsNullOptions_ReturnsUnauthorized()
    {
        var user = new TestUser
        {
            Id = Guid.NewGuid(),
            Username = "add_credential_test_1",
        };

        await _testUserStore.CreateAsync(user);

        _mockCustomizeRegisterOptions
            .Setup(s => s.CustomizeAsync(It.IsAny<CustomizeRegisterOptionsContext>(), CancellationToken.None))
            .Callback<CustomizeRegisterOptionsContext, CancellationToken>((context, _) => context.Options = null);

        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(user.Id),
            CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task AddCredentialAsync_AppRequiresEmail_EmailIsNull_ReturnsUnauthorized()
    {
        var user = new TestUser
        {
            Id = Guid.NewGuid(),
            Username = "username_with_email",
            Email = null!,
        };

        await _testUserStore.CreateAsync(user);

        var identityOptions = new IdentityOptions();
        identityOptions.User.RequireUniqueEmail = true;

        _mockServiceProvider
            .Setup(s => s.GetService(typeof(IOptions<IdentityOptions>)))
            .Returns(Options.Create(identityOptions));

        _mockPasswordlessClient.Setup(s => s.CreateRegisterTokenAsync(It.Is<RegisterOptions>(o
            => o.UserId == user.Id.ToString() && o.Username == "username_with_email" && o.Aliases != null && o.Aliases.Contains("test@email.com")), default))
            .ReturnsAsync(new RegisterTokenResponse("test_email_token"));

        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(user.Id),
            CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    public static IEnumerable<object[]> UnauthenticatedClaimsPrincipals()
    {
        yield return [new ClaimsPrincipal()]; // Empty claims principal with no identity
        yield return [CreateClaimsPrincipal(null, authenticationType: null)]; // Claims principal with one identity that is not authenticated
    }

    [Theory]
    [MemberData(nameof(UnauthenticatedClaimsPrincipals))]
    public async Task AddCredentialAsync_UserNotAuthenticated_ReturnsUnauthorized(ClaimsPrincipal claimsPrincipal)
    {
        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            claimsPrincipal,
            CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task AddCredentialAsync_UserCanNotBeFound_ReturnsUnauthorized()
    {
        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(Guid.NewGuid()),
            CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task AddCredentialAsync_UserIdCanNotBeFound_ReturnsUnauthorized()
    {
        var sut = CreateSut();

        var result = await sut.AddCredentialAsync(
            new PasswordlessAddCredentialRequest("My Test Key"),
            CreateClaimsPrincipal(null),
            CancellationToken.None);

        Assert.IsAssignableFrom<UnauthorizedHttpResult>(result);
    }
}

public sealed class TestUserStore : IUserEmailStore<TestUser>
{
    public Dictionary<Guid, TestUser> InnerStore { get; } = new();

    public Task<IdentityResult> CreateAsync(TestUser user, CancellationToken cancellationToken = default)
    {
        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
        }
        InnerStore.Add(user.Id, user);
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(TestUser user, CancellationToken cancellationToken = default)
    {
        var didRemove = InnerStore.Remove(user.Id);
        var result = didRemove ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
        {
            Code = "error",
            Description = "Could not delete",
        });

        return Task.FromResult(result);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<TestUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var user = InnerStore.Values.FirstOrDefault(u => u.Email.Equals(normalizedEmail, StringComparison.InvariantCultureIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<TestUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!InnerStore.TryGetValue(Guid.Parse(userId), out var user))
        {
            return Task.FromResult<TestUser?>(null);
        }

        return Task.FromResult<TestUser?>(user);
    }

    public Task<TestUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var user = InnerStore.Values.FirstOrDefault(u => string.Equals(u.Username, normalizedUserName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<string?> GetEmailAsync(TestUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult<string?>(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(TestUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedEmailAsync(TestUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedUserNameAsync(TestUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(TestUser user, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(TestUser user, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string?>(user.Username);
    }

    public Task SetEmailAsync(TestUser user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email!;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(TestUser user, bool confirmed, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedEmailAsync(TestUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(TestUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SetUserNameAsync(TestUser user, string? userName, CancellationToken cancellationToken = default)
    {
        user.Username = userName!;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(TestUser user, CancellationToken cancellationToken = default)
    {
        InnerStore[user.Id] = user;
        return Task.FromResult(IdentityResult.Success);
    }
}

public class TestUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}