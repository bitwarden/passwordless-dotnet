# Passwordless .NET SDK

The official [Bitwarden Passwordless.dev](https://passwordless.dev/) .NET library, supporting .NET Standard 2.0+, .NET Core 2.0+ (and soon .NET Framework 4.6.1+).

## Installation

[Nuget Package][nuget-package]  

Using the [.NET Core command-line interface (CLI) tools][dotnet-core-cli-tools]:

```sh
dotnet add package Passwordless
```

Using the [NuGet Command Line Interface (CLI)][nuget-cli]:

```sh
nuget install Passwordless
```

Using the [Package Manager Console][package-manager-console]:

```powershell
Install-Package Passwordless
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "Passwordless".
5. Click on the Passwordless package, select the appropriate version in the
   right-tab and click *Install*.


## Getting started

Follow the [Get started guide](https://docs.passwordless.dev/guide/get-started.html)

#### Register to Dependency Injection

```csharp

// in Program.cs or Startup.cs
services.AddPasswordlessSdk(options =>
{
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

   // Options to give the Api
   var payload = new RegisterOptions
   {
      UserId = userId, // your user id
      Username = alias, // e.g. user email, is shown in browser ui
      Aliases = new HashSet<string> { alias } // Optional: Link this userid to an alias (e.g. email)
   };

   try
   {
      var token = await _passwordlessClient.CreateRegisterTokenAsync(payload);

      // return this token to the frontend
      return Ok(token);
   }
   catch (PasswordlessApiException e)
   {
      return new JsonResult(e.Details)
      {
            StatusCode = (int)e.StatusCode,
      };
   }
}

```

### Verify user

```csharp
[HttpGet]
[Route("/verify-signin")]
public async Task<IActionResult> VerifySignInToken(string token)
{
   try
   {
      var verifiedUser = await _passwordlessClient.VerifyTokenAsync(token);
      // Sign the user in, set a cookie, etc,
      return Ok(verifiedUser);
   }
   catch (PasswordlessApiException e)
   {
      return new JsonResult(e.Details)
      {
            StatusCode = (int)e.StatusCode
      };
   }
}
```



## Documentation

For a comprehensive list of examples, check out the [API
documentation][api-docs]. 


[nuget-package]:https://www.nuget.org/packages/Passwordless/
[api-docs]:https://docs.passwordless.dev/guide/get-started.html
[dotnet-core-cli-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/
[nuget-cli]: https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference
[package-manager-console]: https://docs.microsoft.com/en-us/nuget/tools/package-manager-console
