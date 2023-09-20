// Uncomment line while running API in mock mode to run tests
// #define RUNNING_API

namespace Passwordless.Tests;

public class ApiFactAttribute : FactAttribute
{
    public ApiFactAttribute()
    {
#if !RUNNING_API
        Skip = "These tests are skipped unless you are running the API locally.";
#endif
    }
}