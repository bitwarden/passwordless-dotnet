using Microsoft.Extensions.Options;
using Passwordless.Net;

// This is a trick to always show up in a class when people are registering services
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPasswordlessSdk(this IServiceCollection services, Action<PasswordlessOptions> configureOptions)
    {
        services.AddOptions<PasswordlessOptions>()
            .Configure(configureOptions)
            .PostConfigure(options => options.ApiUrl ??= PasswordlessOptions.CloudApiUrl)
            .Validate(options => !string.IsNullOrEmpty(options.ApiSecret), "Passwordless: Missing ApiSecret");

        services.AddPasswordlessClientCore((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<PasswordlessOptions>>().Value;

            client.BaseAddress = new Uri(options.ApiUrl);
            client.DefaultRequestHeaders.Add("ApiSecret", options.ApiSecret);
        }).AddHttpMessageHandler<PasswordlessDelegatingHandler>();
        return services;
    }

    /// <summary>
    /// Not intended for public use.
    /// </summary>
    /// <remarks>
    /// This method signature is subject to change without major version bump/announcement.
    /// </remarks>
    public static IHttpClientBuilder AddPasswordlessClientCore(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient)
    {
        services.AddTransient<PasswordlessDelegatingHandler>();
        services.AddTransient<IPasswordlessClient, PasswordlessClient>();

        return services.AddHttpClient<IPasswordlessClient, PasswordlessClient>(configureClient);
    }
}