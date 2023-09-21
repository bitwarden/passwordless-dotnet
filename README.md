# <img src="favicon.png" width="24" height="24" alt="Icon" /> Passwordless .NET SDK

The official [Bitwarden Passwordless.dev](https://passwordless.dev) .NET library, supporting .NET Standard 2.0+, .NET Core 2.0+, and .NET Framework 4.6.2+.

## Installation

Install the [NuGet Package](https://nuget.org/packages/Passwordless):

- Using the [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools):

    ```sh
    dotnet add package Passwordless
    ```

- Using the [NuGet CLI](https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference):

    ```sh
    nuget install Passwordless
    ```

- Using the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

    ```powershell
    Install-Package Passwordless
    ```

- From within Visual Studio:

  1. Open the Solution Explorer.
  2. Right-click on a project within your solution.
  3. Click on *Manage NuGet Packages...*
  4. Click on the *Browse* tab and search for "Passwordless".
  5. Click on the Passwordless package, select the appropriate version in the
     right-tab and click *Install*.

## Getting started

💡 See the full [Getting started guide](https://docs.passwordless.dev/guide/get-started.html) in the official documentation.

#### Register using Dependency Injection

```csharp
// In Program.cs or Startup.cs
services.AddPasswordlessSdk(options =>
{
    options.ApiKey = "your_api_key";
    options.ApiSecret = "your_api_secret";
});

```
### Register a passkey

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
