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
    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configure">Configures the <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless<TUser>(this IServiceCollection services, Action<PasswordlessAspNetCoreOptions> configure)
        where TUser : class, new()
    {
        return services.AddPasswordlessCore(typeof(TUser), configure, defaultScheme: null);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="builder">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configure">Configures the <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IdentityBuilder" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IdentityBuilder AddPasswordless(this IdentityBuilder builder, Action<PasswordlessAspNetCoreOptions> configure)
    {
        builder.Services.AddPasswordlessCore(builder.UserType, configure, IdentityConstants.ApplicationScheme);
        return builder;
    }

    [RequiresDynamicCode("Calls Microsoft.Extensions.DependencyInjection.IdentityBuilderExtensions.AddShared(Type, OptionsBuilder<PasswordlessAspNetCoreOptions>, String)")]
    [RequiresUnreferencedCode("Calls Microsoft.Extensions.DependencyInjection.IdentityBuilderExtensions.AddShared(Type, OptionsBuilder<PasswordlessAspNetCoreOptions>, String)")]
    private static IServiceCollection AddPasswordlessCore(this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type userType,
        Action<PasswordlessAspNetCoreOptions> configure,
        string? defaultScheme)
    {
        var optionsBuilder = services.AddOptions<PasswordlessAspNetCoreOptions>().Configure(configure);

        // Add the SDK services but don't configure it there since ASP.NET Core options are a superset of their options.
        services.AddPasswordlessSdk(_ => { });

        // Override SDK options to come from ASP.NET Core options
        services.AddOptions<PasswordlessOptions>()
            .Configure<IOptions<PasswordlessAspNetCoreOptions>>((options, aspNetCoreOptionsAccessor) =>
            {
                var aspNetCoreOptions = aspNetCoreOptionsAccessor.Value;
                options.ApiUrl = aspNetCoreOptions.ApiUrl;
                options.ApiSecret = aspNetCoreOptions.ApiSecret;
                options.ApiKey = aspNetCoreOptions.ApiKey;
            });

        return services.AddShared(userType, optionsBuilder, defaultScheme);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="builder">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> to use to bind to <see cref="PasswordlessAspNetCoreOptions" />. Generally it's own section.</param>
    /// <returns>The <see cref="IdentityBuilder" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless(this IdentityBuilder builder, IConfiguration configuration)
    {
        return builder.AddPasswordless(configuration, builder.UserType, IdentityConstants.ApplicationScheme);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="builder">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="configuration">The <see cref="IConfiguration" /> to use to bind to <see cref="PasswordlessAspNetCoreOptions" />. Generally it's own section.</param>
    /// <returns>The <see cref="IdentityBuilder" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless<TUserType>(this IdentityBuilder builder, IConfiguration configuration)
    {
        return builder.AddPasswordless(configuration, typeof(TUserType), null);
    }

    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    private static IServiceCollection AddPasswordless(this IdentityBuilder builder, IConfiguration configuration, Type? userType, string? defaultScheme)
    {
        var optionsBuilder = builder.Services
            .AddOptions<PasswordlessAspNetCoreOptions>()
            .Bind(configuration);

        builder.Services.AddPasswordlessSdk(configuration);

        return builder.Services.AddShared(userType ?? builder.UserType, optionsBuilder, IdentityConstants.ApplicationScheme);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="builder">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="path">The configuration path to use to bind to <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless(this IdentityBuilder builder, string path)
    {
        return builder.AddPasswordless(path, builder.UserType, IdentityConstants.ApplicationScheme);
    }

    /// <summary>
    /// Adds the services to support <see cref="PasswordlessApiEndpointRouteBuilderExtensions.MapPasswordless(IEndpointRouteBuilder)" />
    /// </summary>
    /// <param name="builder">The current <see cref="IdentityBuilder" /> instance.</param>
    /// <param name="path">The configuration path to use to bind to <see cref="PasswordlessAspNetCoreOptions" />.</param>
    /// <returns>The <see cref="IServiceCollection" />.</returns>
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    public static IServiceCollection AddPasswordless<TUserType>(this IdentityBuilder builder, string path)
    {
        return builder.AddPasswordless(path, typeof(TUserType), null);
    }

    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    private static IServiceCollection AddPasswordless(this IdentityBuilder builder, string path, Type userType, string? defaultScheme)
    {
        var optionsBuilder = builder.Services
            .AddOptions<PasswordlessAspNetCoreOptions>()
            .BindConfiguration(path);

        builder.Services.AddPasswordlessSdk(path);

        return builder.Services.AddShared(userType ?? builder.UserType, optionsBuilder, IdentityConstants.ApplicationScheme);
    }

    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    private static IServiceCollection AddShared(this IServiceCollection services,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type userType,
        OptionsBuilder<PasswordlessAspNetCoreOptions> optionsBuilder,
        string? defaultScheme)
    {
        if (!string.IsNullOrEmpty(defaultScheme))
        {
            optionsBuilder.Configure(o => o.SignInScheme = defaultScheme);
        }

        services.TryAddScoped(
            typeof(IPasswordlessService<PasswordlessRegisterRequest>),
            typeof(PasswordlessService<>).MakeGenericType(userType));

        services.TryAddScoped<ICustomizeRegisterOptions, NoopCustomizeRegisterOptions>();

        return services;
    }
}