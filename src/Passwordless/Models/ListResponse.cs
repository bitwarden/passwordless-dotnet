using System.Collections.Generic;

namespace Passwordless.Models;

internal record ListResponse<T>(IReadOnlyList<T> Values);