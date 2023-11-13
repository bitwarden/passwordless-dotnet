# Passwordless .NET SDK

The official [Bitwarden Passwordless.dev](https://passwordless.dev) .NET library, supporting .NET Standard 2.0+, .NET Core 2.0+, and .NET Framework 4.6.2+.

## Install

- [NuGet](https://nuget.org/packages/Passwordless): `dotnet add package Passwordless`

## See also

Integration packages:

- [Passwordless.AspNetCore](src/Passwordless.AspNetCore) — Passwordless.dev integration with ASP.NET Core Identity

Examples:

- [Passwordless.Example](examples/Passwordless.Example) — basic Passwordless.dev integration inside an ASP.NET app
- [Passwordless.AspNetIdentity.Example](examples/Passwordless.AspNetIdentity.Example) — Passwordless.dev integration using ASP.NET Identity.

## Usage

💡 See the full [Getting started guide](https://docs.passwordless.dev/guide/get-started.html) in the official documentation.

### Resolve the client

Add Passwordless to your service container:

```csharp
// In Program.cs or Startup.cs
services.AddPasswordlessSdk(options =>
{
    options.ApiKey = "your_api_key";
    options.ApiSecret = "your_api_secret";
});
```

Inject the client into your controller:

```csharp
public class HomeController : Controller
{
    private readonly IPasswordlessClient _passwordlessClient;

    public HomeController(IPasswordlessClient passwordlessClient)
    {
        _passwordlessClient = passwordlessClient;
    }
    
    // ...
}
```

### Register a passkey

Define an action or an endpoint to generate a registration token:

```csharp
[HttpGet("/create-token")]
public async Task<IActionResult> GetRegisterToken(string alias)
{
    // Get existing userid from session or create a new user in your database
    var userId = Guid.NewGuid().ToString();
    
    // Provide the userid and an alias to link to this user
    var payload = new RegisterOptions(userId, alias)
    {
        // Optional: Link this userid to an alias (e.g. email)
        Aliases = new HashSet<string> { alias }
    };
    
    try
    {
        var tokenRegistration = await _passwordlessClient.CreateRegisterTokenAsync(payload);
    
        // Return this token to the frontend
        return Ok(tokenRegistration);
    }
    catch (PasswordlessApiException e)
    {
        return new JsonResult(e.Details)
        {
            StatusCode = (int?)e.StatusCode,
        };
    }
}
```

### Verify user

Define an action or an endpoint to verify a signin token:

```csharp
[HttpGet("/verify-signin")]
public async Task<IActionResult> VerifySignInToken(string token)
{
    try
    {
        var verifiedUser = await _passwordlessClient.VerifyTokenAsync(token);

        // Sign the user in, set a cookie, etc
        return Ok(verifiedUser);
    }
    catch (PasswordlessApiException e)
    {
        return new JsonResult(e.Details)
        {
            StatusCode = (int?)e.StatusCode
        };
    }
}
```
