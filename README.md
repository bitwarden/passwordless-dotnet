# Passwordless .NET SDK

[![Build](https://img.shields.io/github/actions/workflow/status/bitwarden/passwordless-dotnet/main.yml?branch=main)](https://github.com/bitwarden/passwordless-dotnet/actions)
[![Coverage](https://img.shields.io/codecov/c/github/bitwarden/passwordless-dotnet/main)](https://codecov.io/gh/bitwarden/passwordless-dotnet)
[![Version](https://img.shields.io/nuget/v/Passwordless.svg)](https://nuget.org/packages/Passwordless)
[![Downloads](https://img.shields.io/nuget/dt/Passwordless.svg)](https://nuget.org/packages/Passwordless)

The official [Bitwarden Passwordless.dev](https://passwordless.dev) .NET library, supporting .NET Standard 2.0+, .NET Core 2.0+, and .NET Framework 4.6.2+.

## Install

- [NuGet](https://nuget.org/packages/Passwordless): `dotnet add package Passwordless`

## See also

Integration packages:

- [Passwordless.AspNetCore](src/Passwordless.AspNetCore) — Passwordless.dev integration with ASP.NET Core Identity

Examples:

- [Passwordless.Example](examples/Passwordless.Example) — basic Passwordless.dev integration inside an ASP.NET app
- [Passwordless.AspNetIdentity.Example](examples/Passwordless.AspNetIdentity.Example) — Passwordless.dev integration using ASP.NET Identity.
- [Passwordless.MultiTenancy.Example](examples/Passwordless.AspNetIdentity.Example) — Passwordless.dev integration for multi-tenant applications.

## Usage

💡 See the full [Getting started guide](https://docs.passwordless.dev/guide/get-started.html) in the official documentation.

### Resolve the client

Add Passwordless to your service container:

```csharp
// In Program.cs or Startup.cs
services.AddPasswordlessSdk(options =>
{
    options.ApiSecret = "your_api_secret";
    options.ApiKey = "your_api_key";
});
```

Inject the client into your controller:

```csharp
public class HomeController(IPasswordlessClient passwordlessClient) : Controller
{
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
        Aliases = [alias]
    };
    
    try
    {
        var tokenRegistration = await passwordlessClient.CreateRegisterTokenAsync(payload);
    
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

Define an action or an endpoint to verify an authentication token:

```csharp
[HttpGet("/verify-signin")]
public async Task<IActionResult> VerifyAuthenticationToken(string token)
{
    try
    {
        var verifiedUser = await passwordlessClient.VerifyTokenAsync(token);

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
