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
using JsonContext = Passwordless.Helpers.PasswordlessSerializerContext;

namespace Passwordless;

/// <inheritdoc cref="IPasswordlessClient" />
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class PasswordlessClient : IPasswordlessClient, IDisposable
{
    private static readonly string SdkVersion =
        typeof(PasswordlessClient).Assembly.GetName().Version?.ToString(3) ??
        // This should never happen, unless the assembly had its metadata trimmed
        "unknown";

    private readonly HttpClient _http;
    private readonly PasswordlessOptions _options;

    private PasswordlessClient(HttpClient http, bool disposeClient, PasswordlessOptions options)
    {
        _http = new HttpClient(new PasswordlessHttpHandler(http, disposeClient), true)
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

        _options = options;
    }

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

    /// <inheritdoc />
    public async Task SetAliasAsync(SetAliasRequest request, CancellationToken cancellationToken)
    {
        using var response = await _http.PostAsJsonAsync("alias",
            request,
            PasswordlessSerializerContext.Default.SetAliasRequest,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task<RegisterTokenResponse> CreateRegisterTokenAsync(RegisterOptions registerOptions, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("register/token",
            registerOptions,
            PasswordlessSerializerContext.Default.RegisterOptions,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync(
            PasswordlessSerializerContext.Default.RegisterTokenResponse,
            cancellationToken))!;
    }

    /// <inheritdoc />
    public async Task<VerifiedUser> VerifyTokenAsync(string verifyToken, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("signin/verify",
            new VerifyTokenRequest(verifyToken),
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
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("users/delete",
            new DeleteUserRequest(userId),
            PasswordlessSerializerContext.Default.DeleteUserRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PasswordlessUserSummary>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            "users/list",
            PasswordlessSerializerContext.Default.ListResponsePasswordlessUserSummary,
            cancellationToken
        );

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AliasPointer>> ListAliasesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            $"alias/list?userid={userId}",
            PasswordlessSerializerContext.Default.ListResponseAliasPointer,
            cancellationToken
        );

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Credential>> ListCredentialsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            $"credentials/list?userid={userId}",
            PasswordlessSerializerContext.Default.ListResponseCredential,
            cancellationToken
        );

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task DeleteCredentialAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("credentials/delete",
            new DeleteCredentialRequest(id),
            PasswordlessSerializerContext.Default.DeleteCredentialRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public async Task DeleteCredentialAsync(byte[] id, CancellationToken cancellationToken = default) =>
        await DeleteCredentialAsync(Base64Url.Encode(id), cancellationToken);

    /// <inheritdoc />
    public async Task<UsersCount> GetUsersCountAsync(CancellationToken cancellationToken = default) =>
        (await _http.GetFromJsonAsync(
            "users/count",
            PasswordlessSerializerContext.Default.UsersCount,
            cancellationToken))!;

    private string DebuggerToString()
    {
        var sb = new StringBuilder();

        sb.Append("ApiUrl = ");
        sb.Append(_options.ApiUrl);

        if (!string.IsNullOrEmpty(_options.ApiSecret))
        {
            if (_options.ApiSecret.Length > 5)
            {
                sb.Append(' ');
                sb.Append("ApiSecret = ");
                sb.Append("***");
                sb.Append(_options.ApiSecret.Substring(_options.ApiSecret.Length - 4));
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