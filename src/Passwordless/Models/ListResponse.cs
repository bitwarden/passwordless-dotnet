using System.Collections.Generic;

namespace Passwordless.Models;

internal class ListResponse<T>
{
    public ListResponse(IReadOnlyList<T> values)
    {
        Values = values;
    }

    public IReadOnlyList<T> Values { get; }
}