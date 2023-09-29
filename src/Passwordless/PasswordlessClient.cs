using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using Passwordless.Helpers;
using Passwordless.Models;
using JsonContext = Passwordless.Helpers.PasswordlessSerializerContext;

namespace Passwordless;

/// <inheritdoc cref="IPasswordlessClient" />
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class PasswordlessClient : IPasswordlessClient, IDisposable
{
    private readonly HttpClient _http;

    private PasswordlessClient(HttpClient http, bool disposeClient, PasswordlessOptions options)
    {
        _http = new HttpClient(new PasswordlessHttpHandler(http, disposeClient))
        {
            BaseAddress = new Uri(options.ApiUrl),
            DefaultRequestHeaders =
            {
                {"ApiSecret", options.ApiSecret}
            }
        };
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
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task<RegisterTokenResponse> CreateRegisterTokenAsync(RegisterOptions registerOptions, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("register/token",
            registerOptions,
            PasswordlessSerializerContext.Default.RegisterOptions,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync(
            PasswordlessSerializerContext.Default.RegisterTokenResponse,
            cancellationToken))!;
    }

    /// <inheritdoc />
    public async Task<VerifiedUser?> VerifyTokenAsync(string verifyToken, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "signin/verify");

        // TODO: No JsonTypeInfo overload yet?
        request.Content = JsonContent.Create(new VerifyTokenRequest(verifyToken));

        // We just want to return null if there is a problem.
        request.SkipErrorHandling();
        using var response = await _http.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var res = await response.Content.ReadFromJsonAsync(
                PasswordlessSerializerContext.Default.VerifiedUser,
                cancellationToken);

            return res;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("users/delete",
            new DeleteUserRequest(userId),
            PasswordlessSerializerContext.Default.DeleteUserRequest,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PasswordlessUserSummary>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            "users/list",
            PasswordlessSerializerContext.Default.ListResponsePasswordlessUserSummary,
            cancellationToken);

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AliasPointer>> ListAliasesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            $"alias/list?userid={userId}",
            PasswordlessSerializerContext.Default.ListResponseAliasPointer,
            cancellationToken);

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Credential>> ListCredentialsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync(
            $"credentials/list?userid={userId}",
            PasswordlessSerializerContext.Default.ListResponseCredential,
            cancellationToken);

        return response!.Values;
    }

    /// <inheritdoc />
    public async Task DeleteCredentialAsync(string id, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("credentials/delete",
            new DeleteCredentialRequest(id),
            PasswordlessSerializerContext.Default.DeleteCredentialRequest,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteCredentialAsync(byte[] id, CancellationToken cancellationToken = default)
    {
        await DeleteCredentialAsync(Base64Url.Encode(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UsersCount> GetUsersCountAsync(CancellationToken cancellationToken = default)
    {
        return (await _http.GetFromJsonAsync(
            "users/count",
            PasswordlessSerializerContext.Default.UsersCount,
            cancellationToken))!;
    }

    private string DebuggerToString()
    {
        var sb = new StringBuilder();
        sb.Append("ApiUrl = ");
        sb.Append(_http.BaseAddress);
        if (_http.DefaultRequestHeaders.TryGetValues("ApiSecret", out var values))
        {
            var apiSecret = values.First();
            if (apiSecret.Length > 5)
            {
                sb.Append(' ');
                sb.Append("ApiSecret = ");
                sb.Append("***");
                sb.Append(apiSecret.Substring(apiSecret.Length - 4));
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