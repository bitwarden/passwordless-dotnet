using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Microsoft.AspNetCore.Http.Results;

namespace Passwordless.AspNetCore.Services.Implementations;

public class PasswordlessService<TUser> : PasswordlessService<TUser, PasswordlessRegisterRequest>
    where TUser : class, new()
{
    /// <summary>
    /// Initializes an instance of <see cref="PasswordlessService{TUser}" />.
    /// </summary>
    public PasswordlessService(
        IPasswordlessClient passwordlessClient,
        IUserStore<TUser> userStore,
        ILogger<PasswordlessService<TUser, PasswordlessRegisterRequest>> logger,
        IOptions<PasswordlessAspNetCoreOptions> optionsAccessor,
        IUserClaimsPrincipalFactory<TUser> userClaimsPrincipalFactory,
        ICustomizeRegisterOptions customizeRegisterOptions,
        IServiceProvider serviceProvider)
        : base(passwordlessClient, userStore, logger, optionsAccessor, userClaimsPrincipalFactory, customizeRegisterOptions, serviceProvider)
    {
    }
}

public class PasswordlessService<TUser, TRegisterRequest>
    : IPasswordlessService<TRegisterRequest>
    where TUser : class, new()
    where TRegisterRequest : PasswordlessRegisterRequest
{
    private readonly ILogger<PasswordlessService<TUser, TRegisterRequest>> _logger;

    /// <summary>
    /// Initializes an instance of <see cref="PasswordlessService{TUser, TRegisterRequest}" />.
    /// </summary>
    public PasswordlessService(
        IPasswordlessClient passwordlessClient,
        IUserStore<TUser> userStore,
        ILogger<PasswordlessService<TUser, TRegisterRequest>> logger,
        IOptions<PasswordlessAspNetCoreOptions> optionsAccessor,
        IUserClaimsPrincipalFactory<TUser> userClaimsPrincipalFactory,
        ICustomizeRegisterOptions customizeRegisterOptions,
        IServiceProvider serviceProvider)
    {
        PasswordlessClient = passwordlessClient;
        UserStore = userStore;
        _logger = logger;
        UserClaimsPrincipalFactory = userClaimsPrincipalFactory;
        CustomizeRegisterOptions = customizeRegisterOptions;
        Options = optionsAccessor.Value;
        IdentityOptions = serviceProvider.GetService<IOptions<IdentityOptions>>()?.Value;
        AuthenticationOptions = serviceProvider.GetService<IOptions<AuthenticationOptions>>()?.Value;
        UserManager = serviceProvider.GetService<UserManager<TUser>>();
    }

    protected IPasswordlessClient PasswordlessClient { get; }
    protected IUserStore<TUser> UserStore { get; }
    protected IUserClaimsPrincipalFactory<TUser> UserClaimsPrincipalFactory { get; }
    protected ICustomizeRegisterOptions CustomizeRegisterOptions { get; }
    protected PasswordlessAspNetCoreOptions Options { get; }
    protected IdentityOptions? IdentityOptions { get; }
    protected AuthenticationOptions? AuthenticationOptions { get; }
    protected UserManager<TUser>? UserManager { get; }

    /// <inheritdoc />
    public virtual async Task<IResult> AddCredentialAsync(
        PasswordlessAddCredentialRequest request,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = await GetUserIdAsync(claimsPrincipal, cancellationToken);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await UserStore.FindByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                _logger.LogDebug("Could not find user with id {UserId} while attempting to add credential", userId);
                return Unauthorized();
            }

            _logger.LogInformation("Found user {UserId} while attempting to add credential", userId);

            var username = await UserStore.GetUserNameAsync(user, cancellationToken);

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            UserInformation userInformation;
            if (IdentityOptions?.User.RequireUniqueEmail is true && UserStore is IUserEmailStore<TUser> emailStore)
            {
                var email = await emailStore.GetEmailAsync(user, cancellationToken);

                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized();
                }

                userInformation = new UserInformation(username, request.DisplayName, new HashSet<string> { email });
            }
            else
            {
                userInformation = new UserInformation(username, request.DisplayName, null);
            }

            var registerOptions = CreateRegisterOptions(userId, userInformation);

            // I could check if the customize service is the noop one and not allocate this context class
            // which would make this a bit more pay-for-play.
            var customizeContext = new CustomizeRegisterOptionsContext(false, registerOptions);
            await CustomizeRegisterOptions.CustomizeAsync(customizeContext, cancellationToken);

            if (customizeContext.Options is null)
            {
                return Unauthorized();
            }

            var registerTokenResponse =
                await PasswordlessClient.CreateRegisterTokenAsync(customizeContext.Options, cancellationToken);

            _logger.LogDebug("Successfully created a register token for user {UserId}", userId);

            return Ok(registerTokenResponse);
        }
        // Route Passwordless API errors to the corresponding result
        catch (PasswordlessApiException ex)
        {
            _logger.LogDebug(
                "Passwordless API responded with an error while attempting to add credential: {Error}",
                ex.Details
            );

            return Problem(
                ex.Details.Detail,
                ex.Details.Instance,
                ex.Details.Status,
                ex.Details.Title,
                ex.Details.Type
            );
        }
    }

    protected virtual ValueTask<string?> GetUserIdAsync(
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        // Do we want to check if the Identity is authenticated? They could have multiple identities and the first one isn't authenticated
        if (claimsPrincipal.Identity?.IsAuthenticated is not true)
        {
            return ValueTask.FromResult<string?>(null);
        }

        // First try our own options, fallback to built in Identity options
        // and then fallback to ClaimsIdentity default
        var userIdClaim = Options.UserIdClaimType
            ?? IdentityOptions?.ClaimsIdentity.UserIdClaimType
            ?? ClaimTypes.NameIdentifier;

        _logger.LogDebug("Will use {ClaimType} as the claim type to search for the user identifier.", userIdClaim);

        var userId = claimsPrincipal.FindFirstValue(userIdClaim);

        return ValueTask.FromResult(userId);
    }

    /// <inheritdoc />
    public virtual async Task<IResult> RegisterUserAsync(
        TRegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = new TUser();
            var result = await CreateUserAsync(user, request, cancellationToken);

            if (!result.Succeeded)
            {
                return ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));
            }

            var userId = await UserStore.GetUserIdAsync(user, cancellationToken);
            _logger.LogDebug("Registering user with id: {Id}", userId);

            var aliases = request.Aliases ?? new HashSet<string>();

            if (IdentityOptions?.User.RequireUniqueEmail is true && UserStore is IUserEmailStore<TUser> emailStore)
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return ValidationProblem(new Dictionary<string, string[]>
                    {
                        { "invalid_email", new[] { "Email cannot be null or empty." } }
                    });
                }

                aliases.Add(request.Email);
            }
            else if (!string.IsNullOrEmpty(request.Email))
            {
                _logger.LogWarning(
                    "An email was provided for {UserId}, but IdentityOptions.User.RequireUniqueEmail was not set to true so the email will not be used as an alias for the passkey.",
                    userId);
            }

            var registerOptions = CreateRegisterOptions(userId,
                new UserInformation(request.Username, request.DisplayName, aliases));

            // Customize register options
            var customizeContext = new CustomizeRegisterOptionsContext(true, registerOptions);
            await CustomizeRegisterOptions.CustomizeAsync(customizeContext, cancellationToken);

            if (customizeContext.Options is null)
            {
                // Is this the best result?
                return Unauthorized();
            }

            var token = await PasswordlessClient.CreateRegisterTokenAsync(customizeContext.Options, cancellationToken);
            return Ok(token);
        }
        // Route Passwordless API errors to the corresponding result
        catch (PasswordlessApiException ex)
        {
            _logger.LogDebug(
                "Passwordless API responded with an error while attempting to register user: {Error}",
                ex.Details
            );

            return Problem(
                ex.Details.Detail,
                ex.Details.Instance,
                ex.Details.Status,
                ex.Details.Title,
                ex.Details.Type
            );
        }
    }

    /// <inheritdoc />
    public virtual async Task<IResult> LoginUserAsync(
        PasswordlessLoginRequest loginRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var verifiedUser = await PasswordlessClient.VerifyAuthenticationTokenAsync(loginRequest.Token, cancellationToken);

            _logger.LogDebug("Attempting to find user in store by id {UserId}.", verifiedUser.UserId);
            var user = await UserStore.FindByIdAsync(verifiedUser.UserId, cancellationToken);

            if (user is null)
            {
                _logger.LogDebug("Could not find user.");
                return Unauthorized();
            }

            var claimsPrincipal = await UserClaimsPrincipalFactory.CreateAsync(user);

            // First try our own scheme, then optionally try built in options but null is still allowed because it
            // will then fallback to the default scheme.
            var scheme = Options.SignInScheme
                         ?? AuthenticationOptions?.DefaultSignInScheme;

            _logger.LogInformation("Signing in user with scheme {Scheme} and {NumberOfClaims} claims",
                scheme, claimsPrincipal.Claims.Count());

            return SignIn(claimsPrincipal, authenticationScheme: scheme);
        }
        // Route Passwordless API errors to the corresponding result
        catch (PasswordlessApiException ex)
        {
            _logger.LogDebug(
                "Passwordless API responded with an error while attempting to login user: {Error}",
                ex.Details
            );

            return Problem(
                ex.Details.Detail,
                ex.Details.Instance,
                ex.Details.Status,
                ex.Details.Title,
                ex.Details.Type
            );
        }
    }

    protected virtual async Task<IdentityResult> CreateUserAsync(
        TUser user,
        TRegisterRequest request,
        CancellationToken cancellationToken)
    {
        await UserStore.SetUserNameAsync(user, request.Username, cancellationToken);

        // I could inject IUserEmailStore as an optional dependency that would allow them to be different implementations
        // but I don't think it's worth it yet, most implementations have IUserStore alongside it.
        if (UserStore is IUserEmailStore<TUser> emailStore)
        {
            await emailStore.SetEmailAsync(user, request.Email, cancellationToken);
        }

        if (UserManager is not null)
        {
            // If they optionally have UserManager let it do all it's stuff
            return await UserManager.CreateAsync(user);
        }
        else
        {
            return await UserStore.CreateAsync(user, cancellationToken);
        }
    }

    protected virtual RegisterOptions CreateRegisterOptions(
        string userId,
        UserInformation userInformation)
    {
        return new RegisterOptions(userId, userInformation.Username)
        {
            DisplayName = userInformation.DisplayName,
            Aliases = userInformation.Aliases ?? new HashSet<string>(),
            Discoverable = Options.Register.Discoverable,
            UserVerification = Options.Register.UserVerification,
            Attestation = Options.Register.Attestation,
            ExpiresAt = DateTime.UtcNow.Add(Options.Register.Expiration),
            AuthenticatorType = Options.Register.AuthenticationType,
            AliasHashing = Options.Register.AliasHashing,
        };
    }
}