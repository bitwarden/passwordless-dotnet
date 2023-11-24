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

        services.AddHttpClient<IPasswordlessClient, PasswordlessClient>((http, sp) =>
            // Above call to services.AddOptions<...> only registers IOptions<PasswordlessOptions>, not
            // PasswordlessOptions itself, so we need to resolve it manually.
            new PasswordlessClient(http, sp.GetRequiredService<IOptions<PasswordlessOptions>>().Value)
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
        IConfiguration configuration) =>
        services.AddPasswordlessSdk(o =>
        {
            o.ApiUrl = configuration["ApiUrl"] ?? PasswordlessOptions.CloudApiUrl;

            o.ApiSecret =
                configuration["ApiSecret"] ??
                throw new InvalidOperationException("Missing 'ApiSecret' configuration value.");

            o.ApiKey = configuration["ApiKey"];
        });
}