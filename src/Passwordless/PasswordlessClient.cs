using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Passwordless.Helpers;
using Passwordless.Models;

namespace Passwordless;

/// <inheritdoc cref="IPasswordlessClient" />
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class PasswordlessClient(HttpClient http, bool disposeClient, PasswordlessOptions options)
    : IPasswordlessClient, IDisposable
{
    /// <summary>
    /// Initializes an instance of <see cref="PasswordlessClient" />.
    /// </summary>
    public PasswordlessClient(HttpClient http, PasswordlessOptions options)
        : this(http, false, options)
    {
    }

    /// <summary>
    /// Initializes an instance of <see cref="PasswordlessClient" />.
    /// </summary>
    public PasswordlessClient(PasswordlessOptions options)
        : this(new HttpClient(), true, options)
    {
    }

    private static readonly string SdkVersion =
        typeof(PasswordlessClient).Assembly.GetName().Version?.ToString(3) ??
        // This should never happen, unless the assembly had its metadata trimmed
        "unknown";

    private readonly HttpClient _http = new(new PasswordlessHttpHandler(http, disposeClient), true)
    {
        BaseAddress = new Uri(options.ApiUrl),
        DefaultRequestHeaders =
        {
            {
                "ApiSecret",
                options.ApiSecret
            },
            {
                "Client-Version",
                $".NET-{SdkVersion}"
            }
        }
    };

    /// <inheritdoc />
    public async Task<RegisterTokenResponse> CreateRegisterTokenAsync(
        RegisterOptions options,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("register/token",
            options,
            PasswordlessSerializerContext.Default.RegisterOptions,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync(
            PasswordlessSerializerContext.Default.RegisterTokenResponse,
            cancellationToken))!;
    }

    /// <inheritdoc />
    public async Task<AuthenticationTokenResponse> GenerateAuthenticationTokenAsync(
        AuthenticationOptions authenticationOptions,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("signin/generate-token",
            authenticationOptions,
            PasswordlessSerializerContext.Default.AuthenticationOptions,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync(
            PasswordlessSerializerContext.Default.AuthenticationTokenResponse,
            cancellationToken))!;
    }

    /// <inheritdoc />
    public async Task<VerifiedUser> VerifyAuthenticationTokenAsync(
        string authenticationToken,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("signin/verify",
            new VerifyTokenRequest(authenticationToken),
            PasswordlessSerializerContext.Default.VerifyTokenRequest,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync(
            PasswordlessSerializerContext.Default.VerifiedUser,
            cancellationToken
        ))!;
    }

    /// <inheritdoc />
    public async Task<GetEventLogResponse> GetEventLogAsync(GetEventLogRequest request, CancellationToken cancellationToken = default) =>
        (await _http.GetFromJsonAsync($"events?pageNumber={request.PageNumber}&numberOfResults={request.NumberOfResults}",
            PasswordlessSerializerContext.Default.GetEventLogResponse,
            cancellationToken))!;

    /// <inheritdoc />
    public async Task<UsersCount> GetUsersCountAsync(CancellationToken cancellationToken = default) =>
        (await _http.GetFromJsonAsync(
            "users/count",
            PasswordlessSerializerContext.Default.UsersCount,
            cancellationToken))!;

    /// <inheritdoc />
    public async Task<IReadOnlyList<PasswordlessUserSummary>> ListUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            "users/list",
            PasswordlessSerializerContext.Default.ListResponsePasswordlessUserSummary,
            cancellationToken
        );

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("users/delete",
            new DeleteUserRequest(userId),
            PasswordlessSerializerContext.Default.DeleteUserRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AliasPointer>> ListAliasesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            $"alias/list?userid={userId}",
            PasswordlessSerializerContext.Default.ListResponseAliasPointer,
            cancellationToken
        );

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task SetAliasAsync(
        SetAliasRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _http.PostAsJsonAsync("alias",
            request,
            PasswordlessSerializerContext.Default.SetAliasRequest,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Credential>> ListCredentialsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            $"credentials/list?userid={userId}",
            PasswordlessSerializerContext.Default.ListResponseCredential,
            cancellationToken
        );

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task DeleteCredentialAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("credentials/delete",
            new DeleteCredentialRequest(id),
            PasswordlessSerializerContext.Default.DeleteCredentialRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task DeleteCredentialAsync(
        byte[] id,
        CancellationToken cancellationToken = default) =>
        await DeleteCredentialAsync(Base64Url.Encode(id), cancellationToken);

    private string DebuggerToString()
    {
        var sb = new StringBuilder();

        sb.Append("ApiUrl = ");
        sb.Append(options.ApiUrl);

        if (!string.IsNullOrEmpty(options.ApiSecret))
        {
            if (options.ApiSecret.Length > 5)
            {
                sb.Append(' ');
                sb.Append("ApiSecret = ");
                sb.Append("***");
                sb.Append(options.ApiSecret.Substring(options.ApiSecret.Length - 4));
            }
        }
        else
        {
            sb.Append(' ');
            sb.Append("ApiSecret = (null)");
        }

        return sb.ToString();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _http.Dispose();
    }
}