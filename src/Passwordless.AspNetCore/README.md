# <img src="../../favicon.png" width="24" height="24" alt="Icon" /> Passwordless ASP.NET Core Integration 

The official [Bitwarden Passwordless.dev](https://passwordless.dev) ASP.NET Identity integration. Automatically adds endpoints to verify passwordless signin and sign the user in using the existing ASP.NET Identity code.

## Installation

Install the [NuGet Package](https://nuget.org/packages/Passwordless.AspNetCore):

- Using the [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools):

    ```sh
    dotnet add package Passwordless.AspNetCore
    ```

- Using the [NuGet CLI](https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference):

    ```sh
    nuget install Passwordless.AspNetCore
    ```

- Using the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

    ```powershell
    Install-Package Passwordless.AspNetCore
    ```

- From within Visual Studio:

   1. Open the Solution Explorer.
   2. Right-click on a project within your solution.
   3. Click on *Manage NuGet Packages...*
   4. Click on the *Browse* tab and search for "Passwordless.AspNetCore".
   5. Click on the Passwordless.AspNetCore package, select the appropriate version in the
      right-tab and click *Install*.

## Usage

💡 See the full [Getting started guide](https://docs.passwordless.dev/guide/get-started.html) in the official documentation.

### Using Minimal APIs

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<MyUser, MyRole>()
      .AddEntityFrameworkStores<MyDbContext>()
      .AddPasswordless(builder.Configuration.GetRequiredSection("Passwordless"));

var app = builder.Build();

app.MapPasswordless();

app.Run();
```

### Using MVC

```csharp
// In Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddIdentity<MyUser, MyRole>()
      .AddEntityFrameworkStores<MyDbContext>()
      .AddPasswordless(Configuration.GetRequiredSection("Passwordless"));

    services.AddControllers();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapPasswordless();
        endpoints.MapControllers();
    });
}
```
