using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Passwordless;

// This is a trick to always show up in a class when people are registering services
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Service registration extensions for Passwordless.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        Action<OptionsBuilder<PasswordlessOptions>> configureOptions)
    {
        // Options
        configureOptions(
            services.AddOptions<PasswordlessOptions>()
        );

        // Client
        services.AddHttpClient<IPasswordlessClient, PasswordlessClient>((http, sp) =>
            new PasswordlessClient(http, sp.GetRequiredService<IOptionsSnapshot<PasswordlessOptions>>().Value)
        );

        // TODO: Get rid of this service, all consumers should use the interface
        services.AddTransient(sp => (PasswordlessClient)sp.GetRequiredService<IPasswordlessClient>());

        return services;
    }

    /// <summary>
    /// Adds and configures Passwordless-related services.
    /// </summary>
    public static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        Action<PasswordlessOptions> configureOptions) =>
        services.AddPasswordlessSdk(o => o.Configure(configureOptions));

    /// <summary>
    /// Adds and configures Passwordless-related services.
    /// </summary>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
#endif
#if NET7_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
#endif
    public static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services.AddPasswordlessSdk(o => o.Bind(configuration));

    /// <summary>
    /// Adds and configures Passwordless-related services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurationSection"></param>
    /// <returns></returns>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
#endif
#if NET7_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
#endif
    public static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        string configurationSection) =>
        services.AddPasswordlessSdk(o => o.BindConfiguration(configurationSection));
}