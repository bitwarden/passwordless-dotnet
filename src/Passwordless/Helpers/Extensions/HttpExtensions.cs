using System.Net;

namespace Passwordless.Helpers.Extensions;

internal static class HttpExtensions
{
    private class NonDisposableHttpContent : HttpContent
    {
        private readonly HttpContent _content;

        public NonDisposableHttpContent(HttpContent content) => _content = content;

        protected override async Task SerializeToStreamAsync(
            Stream stream,
            TransportContext? context
        ) => await _content.CopyToAsync(stream);

        protected override bool TryComputeLength(out long length)
        {
            length = default;
            return false;
        }
    }

    public static HttpRequestMessage Clone(this HttpRequestMessage request)
    {
        var clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
            // Don't dispose the original request's content
            Content = request.Content is not null
                ? new NonDisposableHttpContent(request.Content)
                : null
        };

        foreach (var header in request.Headers)
            clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (request.Content is not null && clonedRequest.Content is not null)
        {
            foreach (var header in request.Content.Headers)
                clonedRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clonedRequest;
    }
}