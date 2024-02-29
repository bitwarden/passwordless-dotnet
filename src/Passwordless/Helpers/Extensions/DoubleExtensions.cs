using System;

namespace Passwordless.Helpers.Extensions;

internal static class DoubleExtensions
{
    internal static int ToInt(this double value) => Convert.ToInt32(value);
}