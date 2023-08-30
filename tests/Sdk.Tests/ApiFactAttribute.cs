#define RUNNING_API

namespace Passwordless.Net.Tests;

public class ApiFactAttribute : FactAttribute
{
    public ApiFactAttribute()
    {
#if !RUNNING_API
        Skip = "These tests are skipped unless you are running the API locally.";
#endif
    }
}