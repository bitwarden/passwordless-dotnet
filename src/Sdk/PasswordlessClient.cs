using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Passwordless.Net.Models;

namespace Passwordless.Net;

/// <summary>
/// TODO: FILL IN
/// </summary>
[DebuggerDisplay("{DebuggerToString(),nq}")]
public class PasswordlessClient : IPasswordlessClient, IDisposable
{
    private readonly bool _needsDisposing;
    private readonly HttpClient _client;

    public static PasswordlessClient Create(PasswordlessOptions options, IHttpClientFactory factory)
    {
        var client = factory.CreateClient();
        client.BaseAddress = new Uri(options.ApiUrl);
        client.DefaultRequestHeaders.Add("ApiSecret", options.ApiSecret);
        return new PasswordlessClient(client);
    }

    public PasswordlessClient(PasswordlessOptions passwordlessOptions)
    {
        _needsDisposing = true;
        _client = new HttpClient
        {
            BaseAddress = new Uri(passwordlessOptions.ApiUrl),
        };
        _client.DefaultRequestHeaders.Add("ApiSecret", passwordlessOptions.ApiSecret);
    }

    public PasswordlessClient(HttpClient client)
    {
        _needsDisposing = false;
        _client = client;
    }

    /// <inheritdoc/>
    public async Task<RegisterTokenResponse> CreateRegisterTokenAsync(RegisterOptions registerOptions, CancellationToken cancellationToken = default)
    {
        var res = await _client.PostAsJsonAsync("register/token", registerOptions, cancellationToken);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<RegisterTokenResponse>(options: null, cancellationToken))!;
    }

    /// <inheritdoc/>
    public async Task<VerifiedUser?> VerifyTokenAsync(string verifyToken, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "signin/verify")
        {
            Content = JsonContent.Create(new
            {
                token = verifyToken,
            }),
        };

        // We just want to return null if there is a problem.
        request.SkipErrorHandling();
        var response = await _client.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var res = await response.Content.ReadFromJsonAsync<VerifiedUser>();
            return res;
        }

        return null;
    }

    /// <inheritdoc/>
    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        await _client.PostAsJsonAsync("users/delete", new { UserId = userId }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<PasswordlessUserSummary>?> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<ListResponse<PasswordlessUserSummary>>("users/list", cancellationToken);
        return response!.Values;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AliasPointer>> ListAliasesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<ListResponse<AliasPointer>>($"alias/list?userid={userId}", cancellationToken);
        return response!.Values;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Credential>> ListCredentialsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetFromJsonAsync<ListResponse<Credential>>($"credentials/list?userid={userId}", cancellationToken);
        return response!.Values;
    }

    /// <inheritdoc/>
    public async Task DeleteCredentialAsync(string id, CancellationToken cancellationToken = default)
    {
        await _client.PostAsJsonAsync("credentials/delete", new { CredentialId = id }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteCredentialAsync(byte[] id, CancellationToken cancellationToken = default)
    {
        await DeleteCredentialAsync(Base64Url.Encode(id), cancellationToken);
    }

    public async Task<UsersCount> GetUsersCountAsync(CancellationToken cancellationToken = default)
    {
        return (await _client.GetFromJsonAsync<UsersCount>("users/count", cancellationToken))!;
    }

    private string DebuggerToString()
    {
        var sb = new StringBuilder();
        sb.Append("ApiUrl = ");
        sb.Append(_client.BaseAddress);
        if (_client.DefaultRequestHeaders.TryGetValues("ApiSecret", out var values))
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

    public void Dispose()
    {
        if (_needsDisposing)
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _client.Dispose();
        }
    }
}