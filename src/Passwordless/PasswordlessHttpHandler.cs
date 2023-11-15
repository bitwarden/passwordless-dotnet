using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Passwordless.Helpers;
using Passwordless.Helpers.Extensions;

namespace Passwordless;

internal class PasswordlessHttpHandler : HttpMessageHandler
{
    // Externally provided HTTP Client
    private readonly HttpClient _http;
    private readonly bool _disposeClient;

    public PasswordlessHttpHandler(HttpClient http, bool disposeClient = false)
    {
        _http = http;
        _disposeClient = disposeClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage providedRequest,
        CancellationToken cancellationToken)
    {
        // Clone the request to reset its completion status, which is required
        // because we're crossing the boundary between two HTTP clients.
        using var request = providedRequest.Clone();

        var response = await _http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        // Provide nice errors for problem details responses
        if (string.Equals(
                response.Content.Headers.ContentType?.MediaType,
                "application/problem+json",
                StringComparison.OrdinalIgnoreCase)
        )
        {
            var problemDetails = await response.Content.ReadFromJsonAsync(
                PasswordlessSerializerContext.Default.PasswordlessProblemDetails,
                cancellationToken
            );

            if (problemDetails is not null)
                throw new PasswordlessApiException(problemDetails);
        }

        return response;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _disposeClient)
            _http.Dispose();
    }
}