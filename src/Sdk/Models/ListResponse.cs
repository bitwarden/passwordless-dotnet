namespace Passwordless.Net.Models;

public class ListResponse<T>
{
    public ListResponse(IReadOnlyList<T> values)
    {
        Values = values;
    }

    public IReadOnlyList<T> Values { get; }
}