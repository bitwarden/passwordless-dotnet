using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Passwordless.AspNetCore.Tests.Dummy;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<PasswordlessDbContext>();

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<PasswordlessDbContext>()
            .AddPasswordless(builder.Configuration.GetSection("Passwordless"));

        var app = builder.Build();

        app.MapPasswordless(new PasswordlessEndpointOptions
        {
            GroupPrefix = "",
            RegisterPath = "/register",
            LoginPath = "/login"
        });

        app.Run();
    }
}

public partial class Program
{
    private class PasswordlessDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public PasswordlessDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // Random database name to avoid sharing state between tests
            builder.UseInMemoryDatabase($"Test_{Guid.NewGuid():N}");
        }
    }
}