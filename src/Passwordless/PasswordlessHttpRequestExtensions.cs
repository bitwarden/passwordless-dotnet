namespace Passwordless;

internal static class PasswordlessHttpRequestExtensions
{
#if NET5_0_OR_GREATER
    internal static HttpRequestOptionsKey<bool> SkipErrorHandlingOption = new(nameof(SkipErrorHandling));
#elif NET462 || NETSTANDARD2_0
    internal const string SkipErrorHandlingOption = nameof(SkipErrorHandling);
#endif

    internal static HttpRequestMessage SkipErrorHandling(this HttpRequestMessage request, bool skip = true)
    {
#if NET5_0_OR_GREATER
        request.Options.Set(SkipErrorHandlingOption, skip);
#elif NET462 || NETSTANDARD2_0
        request.Properties[SkipErrorHandlingOption] = skip;
#endif
        return request;
    }

    internal static bool ShouldSkipErrorHandling(this HttpRequestMessage request)
    {
#if NET5_0_OR_GREATER
        return request.Options.TryGetValue(SkipErrorHandlingOption, out var doNotErrorHandle) && doNotErrorHandle;
#elif NET462 || NETSTANDARD2_0
        return request.Properties.TryGetValue(SkipErrorHandlingOption, out var shouldSkipOptionObject)
               && shouldSkipOptionObject is true;
#endif
    }
}