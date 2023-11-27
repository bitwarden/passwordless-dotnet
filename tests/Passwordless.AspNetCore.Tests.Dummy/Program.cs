using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
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
            .AddPasswordless(_ => { });

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
    public class PasswordlessDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public PasswordlessDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // For some reason, this is required
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            builder.UseSqlite(connection);
        }
    }
}