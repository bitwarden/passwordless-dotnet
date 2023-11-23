using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Passwordless.AspNetCore.Tests.Dummy;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<PasswordlessDbContext>();

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<PasswordlessDbContext>()
            .AddPasswordless(builder.Configuration.GetSection("Passwordless"));

        var app = builder.Build();

        // Execute migrations
        using var scope = app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PasswordlessDbContext>();
        dbContext.Database.Migrate();

        app.MapPasswordless(new PasswordlessEndpointOptions
        {
            GroupPrefix = "",
            RegisterPath = "/register",
            LoginPath = "/login"
        });

        app.Run();
    }
}