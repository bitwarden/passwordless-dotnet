using System;
using System.Diagnostics.CodeAnalysis;
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
    /// <summary>
    /// Adds and configures Passwordless-related services.
    /// </summary>
    public static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        Action<PasswordlessOptions> configureOptions)
    {
        services.AddOptions<PasswordlessOptions>()
            .Configure(configureOptions)
            .PostConfigure(options => options.ApiUrl ??= PasswordlessOptions.CloudApiUrl)
            .Validate(options => !string.IsNullOrEmpty(options.ApiSecret), "Passwordless: Missing ApiSecret");

        services.RegisterDependencies();

        return services;
    }

    /// <summary>
    /// Adds and configures Passwordless-related services.
    /// </summary>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
#endif
    [RequiresUnreferencedCode()]
    public static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<PasswordlessOptions>()
            .Configure(configuration.Bind)
            .Validate(options => !string.IsNullOrEmpty(options.ApiSecret), "Passwordless: Missing ApiSecret");

        services.RegisterDependencies();

        return services;
    }

    /// <summary>
    /// Adds and configures Passwordless-related services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="section"></param>
    /// <returns></returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("This method is incompatible with native AOT compilation.")]
    [RequiresUnreferencedCode("This method is incompatible with assembly trimming.")]
#endif
    [RequiresUnreferencedCode()]
    public static IServiceCollection AddPasswordlessSdk(
        this IServiceCollection services,
        string section)
    {
        services.AddOptions<PasswordlessOptions>().BindConfiguration(section);

        services.RegisterDependencies();

        return services;
    }

    private static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddHttpClient<IPasswordlessClient, PasswordlessClient>((http, sp) =>
            new PasswordlessClient(http, sp.GetRequiredService<IOptionsSnapshot<PasswordlessOptions>>().Value)
        );

        // TODO: Get rid of this service, all consumers should use the interface
        services.AddTransient(sp => (PasswordlessClient)sp.GetRequiredService<IPasswordlessClient>());
    }
}