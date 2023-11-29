# Passwordless ASP.NET Core Integration 

[![Version](https://img.shields.io/nuget/v/Passwordless.AspNetCore.svg)](https://nuget.org/packages/Passwordless.AspNetCore)
[![Downloads](https://img.shields.io/nuget/dt/Passwordless.AspNetCore.svg)](https://nuget.org/packages/Passwordless.AspNetCore)

The official [Bitwarden Passwordless.dev](https://passwordless.dev) ASP.NET Identity integration. Automatically adds endpoints to verify passwordless signin and sign the user in using the existing ASP.NET Identity code.

## Install

- [NuGet](https://nuget.org/packages/Passwordless.AspNetCore): `dotnet add package Passwordless.AspNetCore`

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

### Using Startup class

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
