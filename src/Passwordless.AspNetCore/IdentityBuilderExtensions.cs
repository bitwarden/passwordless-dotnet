using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Passwordless;
using Passwordless.AspNetCore;
using Passwordless.AspNetCore.Services;
using Passwordless.AspNetCore.Services.Implementations;

// Trick to make it show up where it's more likely to be useful
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Default extensions to <see cref="IServiceCollection"/> and <see cref="IdentityBuilder"/> for <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)"/>.
/// </summary>
public static class IdentityBuilderExtensions
{
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    private static IServiceCollection AddPasswordlessIdentity(
        this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type userType,
        Action<OptionsBuilder<PasswordlessAspNetCoreOptions>> configureOptions,
        string? defaultScheme)
    {
        // Options
        var optionsBuilder = services.AddOptions<PasswordlessAspNetCoreOptions>();
        configureOptions(optionsBuilder);

        // Default scheme
        if (!string.IsNullOrEmpty(defaultScheme))
        {
            optionsBuilder.Configure(o => o.SignInScheme = defaultScheme);
        }

        // Services
        services.TryAddScoped(
            typeof(IPasswordlessService<PasswordlessRegisterRequest>),
            typeof(PasswordlessService<>).MakeGenericType(userType)
        );

        services.TryAddScoped<ICustomizeRegisterOptions, NoopCustomizeRegisterOptions>();

        return services;
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configure">Configures the <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless<TUser>(
        this IServiceCollection services,
        Action<PasswordlessAspNetCoreOptions> configure)
        where TUser : class, new()
    {
        // Don't set up options here because we can't use the provided delegate as it's for a different type
        services.AddPasswordlessSdk(_ => { });

        // Derive core options from ASP.NET Core options
        services.AddOptions<PasswordlessOptions>()
            .Configure<IOptions<PasswordlessAspNetCoreOptions>>((options, aspNetCoreOptionsAccessor) =>
            {
                var aspNetCoreOptions = aspNetCoreOptionsAccessor.Value;
                options.ApiUrl = aspNetCoreOptions.ApiUrl;
                options.ApiSecret = aspNetCoreOptions.ApiSecret;
                options.ApiKey = aspNetCoreOptions.ApiKey;
            });

        return services.AddPasswordlessIdentity(typeof(TUser), o => o.Configure(configure), null);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless<TUser>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TUser : class, new()
    {
        services.AddPasswordlessSdk(configuration);
        return services.AddPasswordlessIdentity(typeof(TUser), o => o.Bind(configuration), null);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless<TUser>(
        this IServiceCollection services,
        string configurationSection)
        where TUser : class, new()
    {
        services.AddPasswordlessSdk(configurationSection);
        return services.AddPasswordlessIdentity(typeof(TUser), o => o.BindConfiguration(configurationSection), null);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="identity">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configure">Configures the <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IdentityBuilder" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IdentityBuilder AddPasswordless(
        this IdentityBuilder identity,
        Action<PasswordlessAspNetCoreOptions> configure)
    {
        // Don't set up options here because we can't use the provided delegate as it's for a different type
        identity.Services.AddPasswordlessSdk(_ => { });

        // Derive core options from ASP.NET Core options
        identity.Services.AddOptions<PasswordlessOptions>()
            .Configure<IOptions<PasswordlessAspNetCoreOptions>>((options, aspNetCoreOptionsAccessor) =>
            {
                var aspNetCoreOptions = aspNetCoreOptionsAccessor.Value;
                options.ApiUrl = aspNetCoreOptions.ApiUrl;
                options.ApiSecret = aspNetCoreOptions.ApiSecret;
                options.ApiKey = aspNetCoreOptions.ApiKey;
            });

        identity.Services.AddPasswordlessIdentity(
            identity.UserType,
            o => o.Configure(configure),
            IdentityConstants.ApplicationScheme
        );

        return identity;
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="identity">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> to use to bind to <see cref="PasswordlessAspNetCoreOptions" />. Generally it's own section.</param>
    /// <returns>The <see cref="IdentityBuilder" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IdentityBuilder AddPasswordless(this IdentityBuilder identity, IConfiguration configuration)
    {
        identity.Services.AddPasswordlessSdk(configuration);

        identity.Services.AddPasswordlessIdentity(
            identity.UserType,
            o => o.Bind(configuration),
            IdentityConstants.ApplicationScheme
        );

        return identity;
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="identity">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> to use to bind to <see cref="PasswordlessAspNetCoreOptions" />. Generally it's own section.</param>
    /// <returns>The <see cref="IdentityBuilder" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IdentityBuilder AddPasswordless<TUserType>(this IdentityBuilder identity, IConfiguration configuration)
    {
        identity.Services.AddPasswordlessSdk(configuration);

        identity.Services.AddPasswordlessIdentity(
            typeof(TUserType),
            o => o.Bind(configuration),
            IdentityConstants.ApplicationScheme
        );

        return identity;
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="identity">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configurationSection">The configuration path to use to bind to <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IdentityBuilder AddPasswordless(this IdentityBuilder identity, string configurationSection)
    {
        identity.Services.AddPasswordlessSdk(configurationSection);

        identity.Services.AddPasswordlessIdentity(
            identity.UserType,
            o => o.BindConfiguration(configurationSection),
            IdentityConstants.ApplicationScheme
        );

        return identity;
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="identity">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configurationSection">The configuration path to use to bind to <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IdentityBuilder AddPasswordless<TUserType>(this IdentityBuilder identity, string configurationSection)
    {
        identity.Services.AddPasswordlessSdk(configurationSection);

        identity.Services.AddPasswordlessIdentity(
            typeof(TUserType),
            o => o.BindConfiguration(configurationSection),
            IdentityConstants.ApplicationScheme
        );

        return identity;
    }
}