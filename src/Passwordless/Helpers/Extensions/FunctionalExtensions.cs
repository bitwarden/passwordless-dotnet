using System;

namespace Passwordless.Helpers.Extensions;

internal static class FunctionalExtensions
{
    internal static TOut Pipe<TIn, TOut>(this TIn input, Func<TIn, TOut> transform) => transform(input);
}