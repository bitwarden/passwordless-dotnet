using System.Diagnostics.CodeAnalysis;
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

        services.AddHttpClient<IPasswordlessClient, PasswordlessClient>();

        // TODO: Get rid of this service, all consumers should use the interface
        services.AddTransient(sp => (PasswordlessClient)sp.GetRequiredService<IPasswordlessClient>());

        return services;
    }
}